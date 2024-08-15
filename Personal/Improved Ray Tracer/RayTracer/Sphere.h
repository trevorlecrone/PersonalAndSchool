#pragma once
#include "SolidSurface.h"
class Sphere : public SolidSurface
{
public:
    Sphere(Vec center_, double radius_, Material mat_) : SolidSurface(center_, mat_), radius(radius_) {};

    virtual double CheckCollision(Ray ray);
    virtual bool CheckShadow(Ray ray, double offset, double max_t);
    virtual Vec GetNormal(Vec point);
    
    double GetRadius();

    double quadratic(double a, double b, double c) {
        double discriminant = (b * b) - (4 * a * c);
        //printf("discriminant: %f\n", discriminant);
        if (discriminant < 0) {
            return -999;
        }
        //if we are tangent
        else if (discriminant == 0) {
            return b / (2 * a);
        }
        else {
            double root;
            if (b >= 0) {
                root = (-b + sqrt(discriminant)) / 2;
            }
            else {
                root = (-b - sqrt(discriminant)) / 2;
            }
            double intersection_1 = root / a;
            double intersection_2 = c / root;
            double t_val = fmin(intersection_1, intersection_2);
            if (t_val > 0) {
                return t_val;
            }
            else {
                return -999;
            }
        }

    }

protected:
    double radius;
};