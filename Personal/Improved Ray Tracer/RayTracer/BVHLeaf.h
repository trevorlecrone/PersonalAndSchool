#pragma once
#include "SolidSurface.h"
#include "RecPrism.h"
#include <vector>
class BVHLeaf
{

public:
    BVHLeaf() : volume(RecPrism()) {
    };

    BVHLeaf(RecPrism volume_, std::vector<SolidSurface*> children_) : volume(volume_), children(children_){
    };

protected:
    RecPrism volume;
    std::vector<SolidSurface*> children;
};