// Fill out your copyright notice in the Description page of Project Settings.

#include "PowerupBaseClass.h"


// Sets default values
APowerupBaseClass::APowerupBaseClass()
{
    // Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
    PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void APowerupBaseClass::BeginPlay()
{
    Super::BeginPlay();

}

// Called every frame
void APowerupBaseClass::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);

    FVector NewLocation = GetActorLocation();
    float DeltaHeight = (FMath::Sin(RunningTime + DeltaTime) - FMath::Sin(RunningTime));
    NewLocation.Z += DeltaHeight * 20.0f;       //Scale our height by a factor of 20
    RunningTime += DeltaTime;
    SetActorLocation(NewLocation);

}

