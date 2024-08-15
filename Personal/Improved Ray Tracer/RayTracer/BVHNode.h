#pragma once
#include "RecPrism.h"
#include "BVHLeaf.h"

class BVHNode
{

public:
    BVHNode() : volume(RecPrism()) {
    };

    BVHNode(RecPrism volume_, std::vector<BVHNode*> nodeChildren_, std::vector<BVHLeaf*> leafChildren_) : volume(volume_), nodeChildren(nodeChildren_), leafChildren(leafChildren_) {
    };

protected:
    RecPrism volume;
    std::vector<BVHNode*> nodeChildren;
    std::vector<BVHLeaf*> leafChildren;
};