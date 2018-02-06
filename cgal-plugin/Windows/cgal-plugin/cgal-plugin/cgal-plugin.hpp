
//
//  cgal_plugin
//  cgal-plugin
//
//  Created by Barnabé Faliu on 06/06/2017.
//  Copyright © 2017 Barnabé Faliu. All rights reserved.
//

#ifndef cgal_plugin_hpp
#define cgal_plugin_hpp

#if defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(_WIN64) || defined(WINAPI_FAMILY)
#define UNITY_INTERFACE_API __stdcall
#define UNITY_INTERFACE_EXPORT __declspec(dllexport)
#endif

#include <CGAL/Exact_predicates_inexact_constructions_kernel.h>
#include <CGAL/Surface_mesh.h>
#include <CGAL/Polygon_mesh_processing/corefinement.h>
#include <CGAL/Polygon_mesh_processing/stitch_borders.h>
//For transformation
#include <CGAL/Surface_mesh_deformation.h>
//Clipper
#include <CGAL/Polygon_mesh_processing/internal/clip.h>

#include <stdio.h>
#include <fstream>
#include <cstring>
#include <string>

#define TEST

extern "C" {
	UNITY_INTERFACE_EXPORT const char* UNITY_INTERFACE_API booleanOperation(char* offFile1, char* offFile2, char* operationName);
	UNITY_INTERFACE_EXPORT const char* UNITY_INTERFACE_API booleanOperationClean(const char* offFile1, const char* transform1, const char* offFile2, const char* operationName);
}

#endif 

