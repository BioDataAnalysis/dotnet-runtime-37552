//
// Developed by:  Mario Emmenlauer (mario@emmenlauer.de)
//                Balanstrasse 43, 81669 Munich
//                http://www.biodataanalysis.de/
//
// With contributions by:
//
//
// Copyright (c) 2014-2017, BioDataAnalysis GmbH
// All Rights Reserved.
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

#ifndef INTEROPTESTNATIVEDATASTRUCTURES_HH
#define INTEROPTESTNATIVEDATASTRUCTURES_HH

#include <exception>
#include <stdexcept>

const size_t cMaxErrorMessageSize = 1024;

enum ErrorCodes : int {
    NO_ERROR = 0,
    STD_EXCEPTION = 1,
};

struct ReturnStatus {
    void setError(const std::exception& vException) {
        mErrorCode = ErrorCodes::STD_EXCEPTION;

        // Copy the exception string to the buffer, up to cMaxErrorMessageSize-1:
        size_t vIdx = 0;
        const char* vErrorMessage = vException.what();
        while ((vIdx < (cMaxErrorMessageSize - 1)) && (vErrorMessage[vIdx] != '\0')) {
            mErrorMessage[vIdx] = vErrorMessage[vIdx];
            ++vIdx;
        }

        // Set the exception message size:
        mErrorMessageSize = vIdx;

        // Fill the rest of the buffer with zeros:
        while (vIdx < cMaxErrorMessageSize) {
            mErrorMessage[vIdx] = '\0';
            ++vIdx;
        }
    }

protected:
    int mErrorCode;
    int mErrorMessageSize;
    unsigned char mErrorMessage[cMaxErrorMessageSize];
};

struct TestStruct {
    int mInteger;
    double mDouble;
    int mStringSize;
    unsigned char mString[cMaxErrorMessageSize];
};

#endif