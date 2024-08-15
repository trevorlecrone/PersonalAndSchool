//3d Solid Surface super class
//Trevor LeCrone 2024
#pragma once
#include <stdlib.h>
#include <math.h>
#include "Surface.h"
#include "RecPrism.h"
#include "Ray.h"
#include "Material.h"

class SolidSurface : Surface {

public:
    SolidSurface() : mat(Material()) {
        center = Vec();
    };
    SolidSurface(Vec center_, Material mat_) : Surface(center_), mat(mat_) {};

    using Surface::CheckCollision;
    using Surface::GetNormal;
    using Surface::GetCenter;
    using Surface::SetCenter;

    Material GetMaterial() {
        return mat;
    }
    void SetMaterial(Material mat_) {
        mat = mat_;
    }

    virtual bool CheckShadow(Ray ray, double offset, double max_t) { return false; };

    virtual bool InVolume(RecPrism volume) { return false; };

    virtual ~SolidSurface() {};

protected:
    using Surface::center;
    Material mat;
};