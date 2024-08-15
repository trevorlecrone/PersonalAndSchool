//3d Surface super class
//Trevor LeCrone 2017
#pragma once
#include <stdlib.h>
#include <math.h>
#include "Ray.h"

class Surface {

public:
    Surface() : center(Vec()) {};
    Surface(Vec center_) : center(center_) {};

    
    Vec GetCenter() {
        return center;
    }
    void SetCenter(Vec c) {
        center = c;
    }

    virtual double CheckCollision(Ray ray){ return  0.0;};

    virtual Vec GetNormal(Vec point) { return Vec(); };

    virtual ~Surface() {};

protected:
    Vec center;
};