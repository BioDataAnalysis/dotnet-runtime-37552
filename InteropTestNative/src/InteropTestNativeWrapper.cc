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

#include "InteropTestNativeWrapper.hh"
#include "InteropTestNativeDataStructures.hh"

#include <csignal>
#include <cstdlib>
#include <exception>
#include <iostream>
#include <sstream>
#include <stdexcept>
#include <string>

bool InteropTestNativeWrapper::test_bool_bytearray(const unsigned int aWidth, const unsigned int aHeight, const unsigned char* aData) {
    if (aWidth != 16384) {
        return false;
    }

    if (aHeight != 16384) {
        return false;
    }

    const size_t vSize = aWidth * aHeight;
    for (size_t vIdx = 0; vIdx < vSize; ++vIdx) {
        if (aData[vIdx] != (unsigned char)vIdx) {
            return false;
        }
    }

    return true;
}
