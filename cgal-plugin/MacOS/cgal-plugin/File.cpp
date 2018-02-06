//
//  File.cpp
//  cgal-plugin
//
//  Created by Barnabé Faliu on 06/06/2017.
//  Copyright © 2017 Barnabé Faliu. All rights reserved.
//

#include "File.hpp"

typedef CGAL::Exact_predicates_inexact_constructions_kernel K;
typedef K::Point_3 Point;
typedef CGAL::Surface_mesh<K::Point_3>             Mesh;
//Mesh descriptors
typedef boost::graph_traits<Mesh>::edge_descriptor edge_descriptor;
typedef boost::graph_traits<Mesh>::face_descriptor face_descriptor;
typedef boost::graph_traits<Mesh>::halfedge_descriptor halfedge_descriptor;
typedef CGAL::Halfedge_around_face_circulator<Mesh> halfedge_around_face_circulator;
typedef boost::graph_traits<Mesh>::vertex_descriptor vertex_descriptor;
typedef boost::graph_traits<Mesh>::vertex_iterator vertex_iterator;

// For transformation
typedef CGAL::Surface_mesh_deformation<Mesh>     Surface_mesh_deformation;
typedef K::Aff_transformation_3  Aff_transformation_3;

namespace PMP = CGAL::Polygon_mesh_processing;
namespace params = PMP::parameters;

float getValue (std::stringstream *ss, const char * transform1)
{
    std::string strTemp;
    *ss <<transform1;
    *ss>>strTemp;
    return atof(strTemp.c_str());
}

extern "C" {
    const char* booleanOperation(char* offFile1, char* offFile2, char* operationName) {
        std::stringstream ss1, ss2;
        ss1 << offFile1;
        ss2 << offFile2;
        Mesh mesh1, mesh2;
        ss1>>mesh1;
        ss2>>mesh2;
        Mesh out;
        CGAL::Polygon_mesh_processing::stitch_borders(mesh1);
        CGAL::Polygon_mesh_processing::stitch_borders(mesh2);
        
        //create a property on edges to indicate whether they are constrained
        Mesh::Property_map<edge_descriptor, bool> is_constrained_map =
        mesh1.add_property_map<edge_descriptor, bool>("e:is_constrained",
                                                      false).first;
        
        bool valid = false;
        if (strcmp(operationName, "union")) {
            valid = PMP::corefine_and_compute_union(mesh1,mesh2, out,
                                                    params::all_default(),
                                                    params::all_default(),
                                                    params::edge_is_constrained_map(is_constrained_map));
        } else if (strcmp(operationName, "intersection")) {
            valid = PMP::corefine_and_compute_intersection(mesh1,mesh2, out,
                                                    params::all_default(),
                                                    params::all_default(),
                                                    params::edge_is_constrained_map(is_constrained_map));
        } else if (strcmp(operationName, "difference")) {
            valid = PMP::corefine_and_compute_difference(mesh1,mesh2, out,
                                                    params::all_default(),
                                                    params::all_default(),
                                                    params::edge_is_constrained_map(is_constrained_map));
        } else {
            return "Unknown operation";
        }
        
        if (valid) {
            std::stringstream ssOut;
            ssOut<<out;
            char * cstr = new char [ssOut.str().length()+1];
            std::strcpy (cstr, ssOut.str().c_str());
            return cstr;
        }
        return "invalid operation";
    }
    
    const char* booleanOperationClean(const char* offFile1, const char* transform1, const char* offFile2, const char* operationName) {
        std::ofstream output;
        //Import meshes
        std::stringstream ss1, ss2;
        ss1 << offFile1;
        ss2 << offFile2;
        Mesh mesh1, mesh2, out;
        ss1>>mesh1;
        ss2>>mesh2;
        
        //Refine meshes
        CGAL::Polygon_mesh_processing::stitch_borders(mesh1);
        CGAL::Polygon_mesh_processing::stitch_borders(mesh2);
        
        /*output.open("matrix-in.txt");
        output << transform1;
        output.close();*/
        //Import cutted mesh transform
        float m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23;
        ss1.clear();
        //Export transformation matrix values
        m00=getValue(&ss1, transform1);
        m01=getValue(&ss1, transform1);
        m02=getValue(&ss1, transform1);
        m03=getValue(&ss1, transform1);
        m10=getValue(&ss1, transform1);
        m11=getValue(&ss1, transform1);
        m12=getValue(&ss1, transform1);
        m13=getValue(&ss1, transform1);
        m20=getValue(&ss1, transform1);
        m21=getValue(&ss1, transform1);
        m22=getValue(&ss1, transform1);
        m23=getValue(&ss1, transform1);
        
        output.open("matrix-out.txt");
        output << m00 << m01 << m02 << m03 << m10 << m11 << m12 << m13 << m20
        << m21 << m22 << m23;
        output.close();
        
        // Create the deformation object
        Surface_mesh_deformation deform_mesh(mesh1);
        
        // Select and insert the vertices of the region of interest (select all vertices in this case)
        vertex_iterator vb, ve;
        boost::tie(vb,ve) = vertices(mesh1);
        deform_mesh.insert_roi_vertices(vb, ve);
        
        // Select and insert the control vertices(select all vertices in this case)
        deform_mesh.insert_control_vertices(vb, ve);
        Aff_transformation_3 aff(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23);
        for (vertex_descriptor v : mesh1.vertices())
        {
            Surface_mesh_deformation::Point pos = mesh1.point(v).transform(aff);
            deform_mesh.set_target_position(v, pos);
        }
        deform_mesh.deform();
        
        output.open("transformed.off");
        output << mesh1;
        output.close();
        
        //create a property on edges to indicate whether they are constrained
        Mesh::Property_map<edge_descriptor, bool> is_constrained_map =
        mesh1.add_property_map<edge_descriptor, bool>("e:is_constrained",
                                                      false).first;
        
        bool valid = false;
        if (strcmp(operationName, "union")==0) {
            valid = PMP::corefine_and_compute_union(mesh1,mesh2, out,
                                                    params::all_default(),
                                                    params::all_default(),
                                                    params::edge_is_constrained_map(is_constrained_map));
        } else if (strcmp(operationName, "intersection")==0) {
            valid = PMP::corefine_and_compute_intersection(mesh1,mesh2, out,
                                                           params::all_default(),
                                                           params::all_default(),
                                                           params::edge_is_constrained_map(is_constrained_map));
        } else if (strcmp(operationName, "difference")==0) {
            valid = PMP::corefine_and_compute_difference(mesh1,mesh2, out,
                                                         params::all_default(),
                                                         params::all_default(),
                                                         params::edge_is_constrained_map(is_constrained_map));
        } else {
            return "Unknown operation";
        }

        
        if (valid) {
            /*output.open("result.off");
            output << out;
            output.close();*/
            std::stringstream ssOut;
            ssOut<<out;
            char * cstr = new char [ssOut.str().length()+1];
            std::strcpy (cstr, ssOut.str().c_str());
            return cstr;
        }
        return "invalid operation";
    }
    
    /*const int  checkIntersection(char* cuttedMeshOff, char* cutMeshOff)
    {
        std::stringstream ss1, ss2;
        ss1 << cuttedMeshOff;
        ss2 << cutMeshOff;
        Mesh cuttedMesh, cutMesh;
        ss1 >> cuttedMesh;
        ss2 >> cutMesh;
        Mesh out;
        CGAL::Polygon_mesh_processing::stitch_borders(cuttedMesh);
        CGAL::Polygon_mesh_processing::stitch_borders(cutMesh);
        
        Tree treeCuttedMesh(faces(cuttedMesh).first, faces(cuttedMesh).second, cuttedMesh);
        
        BOOST_FOREACH(face_descriptor fd, faces(cutMesh)) {
            halfedge_descriptor hd = halfedge(fd, cutMesh);
            halfedge_around_face_circulator hafc(hd, cutMesh), done(hafc);
            do {
                
                Point p1 = get(CGAL::vertex_point, cutMesh, target(*hafc, cutMesh));
                ++hafc;
                Point p2 = get(CGAL::vertex_point, cutMesh, target(*hafc, cutMesh));
                Segment s(p1, p2);
                
                if (treeCuttedMesh.do_intersect(s))
                {
                    return 1;
                }
            } while (hafc != done);
        }
        return 0;
    }*/
    
} // extern "C"
