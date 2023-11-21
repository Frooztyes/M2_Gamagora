// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Boids.h"
#include "BoidGenerator.generated.h"

UCLASS()
class BOID_API ABoidGenerator : public AActor
{
	GENERATED_BODY()

public:
	// Sets default values for this actor's properties
	ABoidGenerator();

	FVector offset;
	float acceleration = 1;

	APawn* player;

	bool isLaunched = false;
	FVector launchPosition;
	UStaticMesh* boidMesh;

	UFUNCTION(BlueprintCallable)
	void Initialize(UMaterial* matLoc, int numBoids, UStaticMesh* staticMesh, FVector offsetPlayer, 
		float moveCloserS = 1000,
		float distanceToNeigh = 0.35f, 
		float catchupVelocity = 10,
		float attractionForce = 10, 
		float _vLim = 50);

	UFUNCTION(BlueprintCallable)
	void Launch(FVector position);

	UFUNCTION(BlueprintCallable)
	void AddBoid();

	UFUNCTION(BlueprintCallable)
	int GetBoidsNumber();

	UPROPERTY(VisibleAnywhere)
	UStaticMeshComponent* VisualMesh;


	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters")
	int NumBoids = 2;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters")
	float moveCloserStrength = 100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters")
	float moveWithStrength = 8.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters")
	float moveAwayDistance = 5.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters")
	float tendToPlaceStrength = 100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters")
	float radiusBorder = 100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters")
	float vLim = 50.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters")
	bool tendToPosition = true;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters")
	bool boundToPosition = false;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters")
	bool limitVelocity = true;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters")
	UMaterial*mat;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Parameters")
	UStaticMesh* mesh;

	UPROPERTY(VisibleAnywhere)
	TArray<ABoids*> boids;

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	void GenerateBoids();
	bool IsInSphere(FVector center, FVector point, float radius);
	FVector MoveCloser(ABoids* currentBoid);
	FVector MoveWith(ABoids* currentBoid);
	FVector MoveAway(ABoids* currentBoid);
	FVector TendToPlace(ABoids* currentBoid);
	FVector BoundPosition(ABoids* currentBoid);
	void LimitVelocity(ABoids* currentBoid);
	void MoveBoids(float DeltaTime);

public:
	// Called every frame
	virtual void Tick(float DeltaTime) override;

};
