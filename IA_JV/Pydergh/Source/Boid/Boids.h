// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#ifndef A_BOIDGEN_H
#define A_BOIDGEN_H
#include "BoidGenerator.h"
#endif
#include "Boids.generated.h"

class ABoidGenerator;

UCLASS()
class BOID_API ABoids : public AActor
{
	GENERATED_BODY()

private:
	float maxVelocity = 20;
	bool faceCamera = false;
	ABoidGenerator* generator;
	bool explosif = false;

public:
	ABoids();
	void Initialize(UMaterial *mat, UStaticMesh*mesh, bool faceCam, bool debugCircle, bool estExplosif, ABoidGenerator * boidGenerator);

	UPROPERTY(VisibleAnywhere)
	UStaticMeshComponent* VisualMesh;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters", meta = (DisplayAfter = "Transform", DisplayPriority = 0))
	bool debugVisible = false;


	UFUNCTION(BlueprintCallable)
	void DestroyOwner();

	UFUNCTION(BlueprintCallable)
	int GetBoidsNum();

	UFUNCTION(BlueprintCallable)
	bool IsExplosif();


protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	UBoxComponent* Collider;

public:
	// Called every frame
	virtual void Tick(float DeltaTime) override;
	void RotateToCamera();
	FVector Velocity;

	UFUNCTION()
	void OnOverlapBegin(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor, class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);

	UFUNCTION()
	void OnOverlapEnd(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor, class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex);
};
