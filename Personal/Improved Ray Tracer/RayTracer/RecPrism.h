#pragma once
#include "Surface.h"
#include "BVHTriangle.h"
#include <vector>

class RecPrism : public Surface
{

public:
    RecPrism() : prismCenter(Vec(0,0,0)), hSizeX(1.0), hSizeY(1.0), hSizeZ(1.0) {
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                for (int k = 0; k < 2; k++) {
                    double x = i == 1 ? prismCenter.x + hSizeX : prismCenter.x - hSizeX;
                    double y = j == 1 ? prismCenter.y + hSizeY : prismCenter.y - hSizeY;
                    double z = k == 1 ? prismCenter.z + hSizeZ : prismCenter.z - hSizeZ;
                    componentVertices.push_back(new Vec(x, y, z));
                }
            }
        }
        center = Vec(0, 0, 0);
        componentTriangles.push_back(new BVHTriangle(*componentVertices[1], *componentVertices[3], *componentVertices[5], center));
        componentTriangles.push_back(new BVHTriangle(*componentVertices[7], *componentVertices[3], *componentVertices[5], center));

        componentTriangles.push_back(new BVHTriangle(*componentVertices[1], *componentVertices[3], *componentVertices[4], center));
        componentTriangles.push_back(new BVHTriangle(*componentVertices[1], *componentVertices[2], *componentVertices[4], center));

        componentTriangles.push_back(new BVHTriangle(*componentVertices[1], *componentVertices[2], *componentVertices[5], center));
        componentTriangles.push_back(new BVHTriangle(*componentVertices[6], *componentVertices[2], *componentVertices[5], center));

        componentTriangles.push_back(new BVHTriangle(*componentVertices[7], *componentVertices[6], *componentVertices[5], center));
        componentTriangles.push_back(new BVHTriangle(*componentVertices[7], *componentVertices[6], *componentVertices[8], center));

        componentTriangles.push_back(new BVHTriangle(*componentVertices[4], *componentVertices[2], *componentVertices[6], center));
        componentTriangles.push_back(new BVHTriangle(*componentVertices[4], *componentVertices[6], *componentVertices[8], center));

        componentTriangles.push_back(new BVHTriangle(*componentVertices[3], *componentVertices[4], *componentVertices[7], center));
        componentTriangles.push_back(new BVHTriangle(*componentVertices[4], *componentVertices[7], *componentVertices[8], center));
    };

    RecPrism(Vec prismCenter_, double hSizeX_, double hSizeY_, double hSizeZ_, Vec origin) : Surface(origin), prismCenter(prismCenter_), hSizeX(hSizeX_), hSizeY(hSizeY_), hSizeZ(hSizeZ_) {
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                for (int k = 0; k < 2; k++) {
                    double x = i == 1 ? center.x + hSizeX : center.x - hSizeX;
                    double y = j == 1 ? center.y + hSizeY : center.y - hSizeY;
                    double z = k == 1 ? center.z + hSizeZ : center.z - hSizeZ;
                    componentVertices.push_back(new Vec(x, y, z));
                }
            }
        }
        center = Vec(0, 0, 0);
        componentTriangles.push_back(new BVHTriangle(*componentVertices[1], *componentVertices[3], *componentVertices[5], center));
        componentTriangles.push_back(new BVHTriangle(*componentVertices[7], *componentVertices[3], *componentVertices[5], center));

        componentTriangles.push_back(new BVHTriangle(*componentVertices[1], *componentVertices[3], *componentVertices[4], center));
        componentTriangles.push_back(new BVHTriangle(*componentVertices[1], *componentVertices[2], *componentVertices[4], center));

        componentTriangles.push_back(new BVHTriangle(*componentVertices[1], *componentVertices[2], *componentVertices[5], center));
        componentTriangles.push_back(new BVHTriangle(*componentVertices[6], *componentVertices[2], *componentVertices[5], center));

        componentTriangles.push_back(new BVHTriangle(*componentVertices[7], *componentVertices[6], *componentVertices[5], center));
        componentTriangles.push_back(new BVHTriangle(*componentVertices[7], *componentVertices[6], *componentVertices[8], center));

        componentTriangles.push_back(new BVHTriangle(*componentVertices[4], *componentVertices[2], *componentVertices[6], center));
        componentTriangles.push_back(new BVHTriangle(*componentVertices[4], *componentVertices[6], *componentVertices[8], center));

        componentTriangles.push_back(new BVHTriangle(*componentVertices[3], *componentVertices[4], *componentVertices[7], center));
        componentTriangles.push_back(new BVHTriangle(*componentVertices[4], *componentVertices[7], *componentVertices[8], center));
    };


    virtual double CheckCollision(Ray ray);

    //we will recycle the center attribute of the surface as the origin we calculate distance from;
protected:
    Vec prismCenter;
    double hSizeX;
    double hSizeY;
    double hSizeZ;
    std::vector<Vec*> componentVertices;
    std::vector<BVHTriangle*> componentTriangles;
};