//
// Developed for BioDataAnalysis GmbH <info@biodataanalysis.de>
//               Balanstrasse 43, 81669 Munich
//               https://www.biodataanalysis.de/
//
// Copyright (c) BioDataAnalysis GmbH. All Rights Reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are not permitted. All information contained herein
// is, and remains the property of BioDataAnalysis GmbH.
// Dissemination of this information or reproduction of this material
// is strictly forbidden unless prior written permission is obtained
// from BioDataAnalysis GmbH.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
//

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;

namespace bda
{
    /// <summary>
    /// </summary>
    public class NativeWrapper
    {
        internal const string cNativeImportName = "InteropTestNative";
        internal const int cMaxErrorMessageSize = 1024;
        private static ILogger mLogger = null;


        // Definition of data structures

        [StructLayout(LayoutKind.Sequential)]
        public struct ReturnStatus
        {
            private int mErrorCode;
            private int mErrorMessageSize;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cMaxErrorMessageSize)]
            private byte[] mErrorMessage;

            public int getErrorCode()
            {
                return mErrorCode;
            }

            public string getErrorMessage()
            {
                return System.Text.Encoding.UTF8.GetString(mErrorMessage, 0, mErrorMessageSize);
            }

            public void rethrowErrorIfAny()
            {
                if (getErrorCode() != 0)
                {
                    throw new Exception(getErrorMessage());
                }
            }

            public static ReturnStatus create()
            {
                ReturnStatus vReturnStatus = new ReturnStatus();
                vReturnStatus.mErrorMessage = new byte[cMaxErrorMessageSize];
                return vReturnStatus;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TestStruct
        {
            public int mInteger;
            public double mDouble;
            public int mStringSize;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cMaxErrorMessageSize)]
            public byte[] mString;

            public string getString()
            {
                return System.Text.Encoding.UTF8.GetString(mString, 0, mStringSize);
            }

            public static TestStruct create()
            {
                TestStruct vTestStruct = new TestStruct();
                vTestStruct.mString = new byte[cMaxErrorMessageSize];

                // Check that we can pass data to managed code:
                for (uint vIdx = 0; vIdx < vTestStruct.mString.Length; ++vIdx)
                {
                    vTestStruct.mString[vIdx] = (byte)(vIdx % 255);
                }

                return vTestStruct;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct FixedArrayTestStruct
        {

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cMaxErrorMessageSize)]
            public fixed byte mString[128];
        }


        [DllImport(cNativeImportName, EntryPoint = "test_native", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void test_native();

        [DllImport(cNativeImportName, EntryPoint = "test_void_void", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void test_void_void();

        [DllImport(cNativeImportName, EntryPoint = "test_bool_void", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern bool test_bool_void();

        [DllImport(cNativeImportName, EntryPoint = "test_bool_bytearray", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern bool test_bool_bytearray(uint aWidth, uint aHeight, [In] byte[] aData);

        [DllImport(cNativeImportName, EntryPoint = "test_bool_string", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern bool test_bool_string([In, Out] char[] aReturnString);

        [DllImport(cNativeImportName, EntryPoint = "test_int_struct", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int test_int_struct(ref TestStruct aTestStruct);

        [DllImport(cNativeImportName, EntryPoint = "triggerStdExceptionStructAPIIntern", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void triggerStdExceptionStructAPIIntern(ref ReturnStatus aReturnStatus);
        public static void triggerStdExceptionStructManaged()
        {
            ReturnStatus vReturnStatus = ReturnStatus.create();
            triggerStdExceptionStructAPIIntern(ref vReturnStatus);
            vReturnStatus.rethrowErrorIfAny();
        }

        [DllImport(cNativeImportName, EntryPoint = "complexStdExceptionStructAPIIntern", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern void complexStdExceptionStructAPIIntern(ref ReturnStatus aReturnStatus, uint aWidth, uint aHeight, [In] byte[] aData);
        public static void complexStdExceptionStructManaged(uint aWidth, uint aHeight, byte[] aData)
        {
            ReturnStatus vReturnStatus = ReturnStatus.create();
            complexStdExceptionStructAPIIntern(ref vReturnStatus, aWidth, aHeight, aData);
            vReturnStatus.rethrowErrorIfAny();
        }

        [DllImport(cNativeImportName, EntryPoint = "complexStdExceptionDirectAPIIntern", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        private static extern int complexStdExceptionDirectAPIIntern([Out] byte[] aErrorMessage, uint aWidth, uint aHeight, [In] byte[] aData);
        public static void complexStdExceptionDirectManaged(uint aWidth, uint aHeight, byte[] aData)
        {
            exceptionPassingExecute((_1) => complexStdExceptionDirectAPIIntern(_1, aWidth, aHeight, aData));
        }


        // This class defines a public test method:
        public static bool test(ILogger aLogger = null)
        {
            mLogger = aLogger;
            try
            {
                test_native();
            }
            catch (Exception vException)
            {
                mLogger?.LogError("NativeWrapper::test(): " + vException.ToString() + "\n");
                return false;
            }
            return true;
        }

        public static void exceptionPassingExecute(Func<byte[], int> aFunc)
        {
            byte[] vErrorMessage = new byte[cMaxErrorMessageSize];
            int vErrorCode = aFunc(vErrorMessage);
            rethrowErrorIfAny(vErrorCode, vErrorMessage);
        }

        public static void rethrowErrorIfAny(int aErrorCode, byte[] aErrorMessage)
        {
            if (aErrorCode != 0)
            {
                throw new Exception(getErrorMessage(aErrorMessage));
            }
        }

        public static string getErrorMessage(byte[] aErrorMessage)
        {
            int vZeroTerminationIndex = Array.IndexOf(aErrorMessage, (byte)0);
            if (vZeroTerminationIndex < 0)
            {
                throw new Exception("getErrorMessage(): The native code has passed an error message without zero termination");
            }
            return System.Text.Encoding.UTF8.GetString(aErrorMessage, 0, vZeroTerminationIndex);
        }

        // This class does not need to be instantiated:
        private NativeWrapper() { }
    }

    /// <summary>
    /// Helper using the NativeLibrary class to aid in resolving native library handles
    /// </summary>
    /// to learn about ways to find the library path
    /// https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.nativelibrary
    /// https://developers.redhat.com/blog/2019/09/06/interacting-with-native-libraries-in-net-core-3-0/
    public class NativeWrapperTest
    {
        private static bool mSuccessfullyInitialized = false;
        private static ILogger mLogger = null;

        public static bool tryInitialize(ILogger aLogger = null)
        {
            mLogger = aLogger;
            if (mSuccessfullyInitialized)
            {
                return true;
            }

            try
            {
                NativeLibrary.SetDllImportResolver(typeof(NativeWrapper).Assembly, ImportResolver);

                mLogger?.LogTrace("NativeWrapperTest::tryInitialize(): Will try to initialize the NativeWrapper.\n");
                if (NativeWrapper.test())
                {
                    mSuccessfullyInitialized = true;
                    return true;
                }
            }
            catch (DllNotFoundException vException)
            {
                mLogger?.LogError("NativeWrapperTest::tryInitialize(): " + vException.ToString() + "\n");
            }
            catch (EntryPointNotFoundException vException)
            {
                mLogger?.LogError("NativeWrapperTest::tryInitialize(): " + vException.ToString() + "\n");
            }
            catch (Exception vException)
            {
                mLogger?.LogError("NativeWrapperTest::tryInitialize(): " + vException.ToString() + "\n");
            }

            mLogger?.LogTrace("NativeWrapperTest::tryInitialize(): Failed to initialize the NativeWrapper, native test method returned 'false'.\n");
            return false;
        }

        private static IntPtr ImportResolver(string aLibraryName, Assembly aAssembly, DllImportSearchPath? aDllImportSearchPath)
        {
            // This is the import resolver for the cNativeImportName only:
            if (aLibraryName == NativeWrapper.cNativeImportName)
            {
                IntPtr vLibraryHandle;
                string vNativeLibraryFile = getNativeInteropTestLibrary();
                if (NativeLibrary.TryLoad(vNativeLibraryFile, out vLibraryHandle))
                {
                    mLogger?.LogTrace("NativeWrapperTest::ImportResolver(): Loading native library '" + vNativeLibraryFile + "' failed.\n");
                    return vLibraryHandle;
                }
            }

            return IntPtr.Zero;
        }

        protected static string getProjectInstallDirectory()
        {
            string vProjectInstallDirectory;
            vProjectInstallDirectory = Environment.GetEnvironmentVariable("BBSTARGETDIR_OSPATH");
            if (vProjectInstallDirectory == null)
            {
                mLogger?.LogInformation("PInvokeTests::getProjectInstallDirectory(): Environment variable 'BBSTARGETDIR_OSPATH' is unset.\n");
                throw new Exception("PInvokeTests::getProjectInstallDirectory(): Environment variable 'BBSTARGETDIR_OSPATH' is unset");
            }

            //vProjectInstallDirectory += "/BDAImageAcquireManaged";
            if (!Directory.Exists(vProjectInstallDirectory))
            {
                mLogger?.LogInformation("PInvokeTests::getProjectInstallDirectory(): 'BBSTARGETDIR_OSPATH' points to non-existing directory '" + vProjectInstallDirectory + "'.\n");
                throw new Exception("PInvokeTests::getProjectInstallDirectory(): 'BBSTARGETDIR_OSPATH' points to non-existing directory '" + vProjectInstallDirectory + "'");
            }

            return vProjectInstallDirectory;
        }

        protected static string getNativeInteropTestLibrary()
        {
            string vProjectInstallDirectory = Directory.GetCurrentDirectory() + "/../../../../../InteropTestNative/bin";
            string vLibraryFile;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                vLibraryFile = vProjectInstallDirectory + "\\bin\\InteropTestNative.dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                vLibraryFile = vProjectInstallDirectory + "/lib/libInteropTestNative.so";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                vLibraryFile = vProjectInstallDirectory + "/lib/libInteropTestNative.dylib";
            }
            else
            {
                mLogger?.LogInformation("PInvokeTests::getNativeInteropTestLibrary(): Current platform not supported.\n");
                throw new Exception("PInvokeTests::getNativeInteropTestLibrary(): Current platform not supported");
            }

            if (!File.Exists(vLibraryFile))
            {
                mLogger?.LogInformation("PInvokeTests::getNativeInteropTestLibrary(): Platform native library '" + vLibraryFile + "' does not exist.\n");
                throw new Exception("PInvokeTests::getNativeInteropTestLibrary(): Platform native library '" + vLibraryFile + "' does not exist");
            }

            return vLibraryFile;
        }

        public static bool IsBlittable<T>()
        {
            return IsBlittableCache<T>.Value;
        }

        public static bool IsBlittable(Type type)
        {
            if (type.IsArray)
            {
                var vTmpArrayElement = type.GetElementType();
                return (vTmpArrayElement.IsValueType && IsBlittable(vTmpArrayElement));
            }
            if (type == typeof(decimal))
            {
                return false;
            }

            try
            {
                object vTmpInstance = FormatterServices.GetUninitializedObject(type);
                GCHandle.Alloc(vTmpInstance, GCHandleType.Pinned).Free();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static class IsBlittableCache<T>
        {
            public static readonly bool Value = IsBlittable(typeof(T));
        }

        // This class does not need to be instantiated:
        private NativeWrapperTest() { }
    }
}
