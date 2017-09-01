#
# Developed for BioDataAnalysis GmbH <info@biodataanalysis.de>
#               Balanstrasse 43, 81669 Munich
#               https://www.biodataanalysis.de/
#
# Copyright (c) BioDataAnalysis GmbH. All Rights Reserved.
#
# Redistribution and use in source and binary forms, with or without
# modification, are not permitted. All information contained herein
# is, and remains the property of BioDataAnalysis GmbH.
# Dissemination of this information or reproduction of this material
# is strictly forbidden unless prior written permission is obtained
# from BioDataAnalysis GmbH.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
#

cmake_minimum_required(VERSION 3.17 FATAL_ERROR)

project(InteropTestNative VERSION 0.1.0 LANGUAGES CXX)

set(BUILD_SHARED_LIBS ON)

list(APPEND CMAKE_MODULE_PATH
    ${CMAKE_PREFIX_PATH})

set(SOURCES
    src/InteropTestNativeWrapper.hh
    src/InteropTestNativeWrapper.cc
    src/InteropTestNativeExport.hh
    src/InteropTestNativeExport.cc)

add_library(${PROJECT_NAME} SHARED ${SOURCES})

target_compile_options(${PROJECT_NAME}
    PRIVATE
        $<$<CXX_COMPILER_ID:GNU>:-pedantic;-Wall;-Wextra;-Werror=return-type;-Werror=uninitialized;-Werror=unknown-pragmas>
        $<$<CXX_COMPILER_ID:MSVC>:/W4;/wd4251;/wd4244;/wd4267;/bigobj>)

set_target_properties(${PROJECT_NAME} PROPERTIES
    CXX_STANDARD 17
    CXX_STANDARD_REQUIRED ON
    CXX_EXTENSIONS NO)

set_target_properties(${PROJECT_NAME} PROPERTIES
    VERSION ${PROJECT_VERSION}
    SOVERSION ${PROJECT_VERSION_MAJOR}
    POSITION_INDEPENDENT_CODE ON
    CXX_VISIBILITY_PRESET hidden
    VISIBILITY_INLINES_HIDDEN ON)

target_include_directories(${PROJECT_NAME}
    PRIVATE
        ${CMAKE_CURRENT_SOURCE_DIR}/src
        ${CMAKE_CURRENT_BINARY_DIR}
    PUBLIC)

include(GenerateExportHeader)
generate_export_header(${PROJECT_NAME} EXPORT_FILE_NAME interoptestnative_export.h)

install(TARGETS ${PROJECT_NAME}
    ARCHIVE DESTINATION lib
    LIBRARY DESTINATION lib
    RUNTIME DESTINATION bin)
