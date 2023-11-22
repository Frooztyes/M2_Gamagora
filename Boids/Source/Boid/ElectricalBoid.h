// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Boids.h"
#include <NiagaraSystem.h>
#include "ElectricalBoid.generated.h"


UCLASS()
class BOID_API AElectricalBoid : public ABoids
{
	GENERATED_BODY()

protected:

	UPROPERTY(EditAnywhere, Category = "Firing")
	UNiagaraSystem* particle;
	USceneComponent* sceneComp;

public:
	virtual void BeginPlay() override;
	AElectricalBoid();

	void Initialize(UMaterial* mat, UStaticMesh* mesh, UNiagaraSystem * nsParticle, bool faceCam, bool debugCircle, ABoidGenerator* boidGenerator);

};
