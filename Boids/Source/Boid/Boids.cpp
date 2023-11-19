// Fill out your copyright notice in the Description page of Project Settings.


#include "Boids.h"
#include <chrono>
#include <Components/SphereComponent.h>

// Sets default values
ABoids::ABoids()
{
	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;


	VisualMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Mesh"));
	VisualMesh->SetupAttachment(RootComponent);

	//static ConstructorHelpers::FObjectFinder<UStaticMesh> 
	//	CubeVisualAsset(TEXT("/Game/StarterContent/Shapes/Shape_Plane.Shape_Plane"));
	static ConstructorHelpers::FObjectFinder<UStaticMesh>
		CubeVisualAsset(TEXT("/Game/StarterContent/Shapes/Shape_Sphere.Shape_Sphere"));

	if (CubeVisualAsset.Succeeded())
	{
		VisualMesh->SetStaticMesh(CubeVisualAsset.Object);
		VisualMesh->SetRelativeLocation(FVector(0.0f, 0.0f, 0.0f));
		VisualMesh->SetCollisionProfileName(TEXT("NoCollision"));
	}

	Velocity = FVector(0, 0, 0);

	FVector origin;
	FVector boxExtent;
	GetActorBounds(false, origin, boxExtent);

	USphereComponent* collider = CreateDefaultSubobject<USphereComponent>(TEXT("Attacking Cullusion"));
	collider->InitSphereRadius(boxExtent.X);
	AddInstanceComponent(collider);
}

void ABoids::Initialize(UMaterial *mat, UStaticMesh* mesh, bool faceCam, bool debugCircle) {
	VisualMesh->SetMaterial(0, mat);
	VisualMesh->SetStaticMesh(mesh);
	this->faceCamera = faceCam;
	this->debugVisible = debugCircle;

}


// Called when the game starts or when spawned
void ABoids::BeginPlay()
{
	Super::BeginPlay();
	using namespace std::chrono;
	milliseconds ms = duration_cast<milliseconds>(
		system_clock::now().time_since_epoch()
	);
	FMath::RandInit(ms.count());
	Velocity.X = FMath::RandRange(1, 10) / 10.0;
	Velocity.Y = FMath::RandRange(1, 10) / 10.0;
	Velocity.Z = FMath::RandRange(1, 10) / 10.0;
}

void ABoids::RotateToCamera() {
	APlayerCameraManager* PlayerCamera = GetWorld()->GetFirstPlayerController()->PlayerCameraManager;
	FVector selfLoc = GetActorLocation();
	FVector camLoc = PlayerCamera->GetCameraLocation();
	FVector dir = (camLoc - selfLoc);
	dir.Normalize(0.0001);
	FRotator rotToCam = dir.Rotation();
	rotToCam.Pitch += 90;
	SetActorRotation(rotToCam);
}

// Called every frame
void ABoids::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	if (faceCamera)
		RotateToCamera();

}

