//
//  File.hpp
//  cgal-plugin
//
//  Created by Barnabé Faliu on 06/06/2017.
//  Copyright © 2017 Barnabé Faliu. All rights reserved.
//

#ifndef File_hpp
#define File_hpp


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
    TEST const char* booleanOperation(char* offFile1, char* offFile2, char* operationName);
    TEST const char* booleanOperationClean(const char* offFile1, const char* transform1, const char* offFile2, const char* operationName);
} // extern "C"

#endif /* File_hpp */

