// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Boids.generated.h"

UCLASS()
class BOID_API ABoids : public AActor
{
	GENERATED_BODY()

private:
	float maxVelocity = 20;
	bool faceCamera = false;

public:
	// Sets default values for this actor's properties
	ABoids();
	void Initialize(UMaterial *mat, UStaticMesh*mesh, bool faceCam, bool debugCircle);

	UPROPERTY(VisibleAnywhere)
	UStaticMeshComponent* VisualMesh;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters", meta = (DisplayAfter = "Transform", DisplayPriority = 0))
	bool debugVisible = false;

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void Tick(float DeltaTime) override;
	void RotateToCamera();
	FVector Velocity;

};
