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
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;

namespace NativeBinding.Tests
{
    [TestClass]
    public class PInvokeTests
    {
        private static ILogger mLogger = null;

        private static ILogger getLogger()
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
            return loggerFactory.CreateLogger("NativeBinding.Tests");
        }

        private static byte[] createTestData(uint aWidth, uint aHeight)
        {
            uint vSize = aWidth * aHeight;
            byte[] vData = new byte[vSize];
            for (uint vIdx = 0; vIdx < vData.Length; ++vIdx)
            {
                vData[vIdx] = (byte)vIdx;
            }
            return vData;
        }

        [TestInitialize]
        public void Initialize()
        {
            mLogger = getLogger();
        }

        [TestMethod]
        [TestCategory("Benchmark")]
        public void NativeLibraryBenchmark()
        {
            if (!bda.NativeWrapperTest.tryInitialize(mLogger))
            {
                Console.WriteLine("Error initializing the native wrapper");
                throw new Exception("Error initializing the native wrapper");
            }

            uint vWidth = 16384;
            uint vHeight = 16384;
            byte[] vData = createTestData(vWidth, vHeight);

            // Make one test call before:
            Assert.IsTrue(bda.NativeWrapper.test_bool_bytearray(vWidth, vHeight, vData));

            ulong vNumCalls = 0;
            ulong vElapsedMilliseconds = 0;
            Stopwatch vStopwatch = Stopwatch.StartNew();
            while (vElapsedMilliseconds <= 5000)
            {
                Assert.IsTrue(bda.NativeWrapper.test_bool_bytearray(vWidth, vHeight, vData));
                ++vNumCalls;
                vElapsedMilliseconds = (ulong)vStopwatch.ElapsedMilliseconds;
            }

            double vBytesTransferred = (ulong)vData.Length * vNumCalls;
            double vMegaBytesTransferred = vBytesTransferred / (1024.0 * 1024.0);
            double vElapsedSeconds = vElapsedMilliseconds / 1000.0;

            string vSummary = "NativeLibraryBenchmark(): ";
            vSummary += "Transferred " + vMegaBytesTransferred + "MB ";
            vSummary += "in " + vElapsedSeconds + "s ";
            vSummary += ", rate " + (vMegaBytesTransferred / vElapsedSeconds) + "MB/s";
            vSummary += ", " + (vNumCalls / vElapsedSeconds) + "/s ";
            Console.WriteLine(vSummary);
        }
    }
}
