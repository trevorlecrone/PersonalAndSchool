#include "RecPrism.h"

double RecPrism::CheckCollision(Ray ray) {
    double collision = -999.0;
    for each (BVHTriangle* t in componentTriangles) {
        collision = t->CheckCollision(ray);
        if (collision > 0.0) {
            return collision;
        }
    }
    return collision;
}