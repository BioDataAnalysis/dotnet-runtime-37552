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

#ifndef INTEROPTESTNATIVEEXPORT_HH
#define INTEROPTESTNATIVEEXPORT_HH

#include "interoptestnative_export.h"

#ifdef __cplusplus
extern "C" {
#endif

INTEROPTESTNATIVE_EXPORT void test_native();
INTEROPTESTNATIVE_EXPORT bool test_bool_bytearray(const unsigned int aWidth, const unsigned int aHeight, const unsigned char* aData);

#ifdef __cplusplus
}
#endif

#endif
