// Fill out your copyright notice in the Description page of Project Settings.


#include "Boids.h"
#include <chrono>
#include <Components/SphereComponent.h>
#include "BP_GroundClass.h"

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
	Collider = NewObject<UBoxComponent>(this, FName("BoxCollider"));
	Collider->SetBoxExtent(VisualMesh->Bounds.BoxExtent);
	Collider->bDynamicObstacle = true;
	Collider->AttachToComponent(VisualMesh, FAttachmentTransformRules::KeepRelativeTransform);
	Collider->SetWorldLocation(GetActorLocation() + FVector(0, 0, VisualMesh->Bounds.BoxExtent.Z));
	Collider->OnComponentBeginOverlap.AddDynamic(this, &ABoids::OnOverlapBegin);
	Collider->OnComponentEndOverlap.AddDynamic(this, &ABoids::OnOverlapEnd);
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
	Velocity.X = FMath::SRand();
	Velocity.Y = FMath::SRand();
	Velocity.Z = FMath::SRand();

	FVector origin;
	FVector boxExtent;
	GetActorBounds(false, origin, boxExtent);
	Collider->SetBoxExtent(boxExtent);
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

void ABoids::OnOverlapBegin(class UPrimitiveComponent* OverlappedComp, 
	class AActor* OtherActor, 
	class UPrimitiveComponent* OtherComp, 
	int32 OtherBodyIndex, 
	bool bFromSweep, 
	const FHitResult& SweepResult) 
{
	//GEngine->AddOnScreenDebugMessage(-1, 1.f, FColor::Yellow, OtherActor->GetClass());
	if (Cast<ABP_GroundClass>(OtherActor)) {
		Destroy();
	}

	GEngine->AddOnScreenDebugMessage(-1, 1.f, FColor::Yellow, "---------------");
	if (OverlappedComp->ComponentHasTag("Ground")) {
	}
}

void ABoids::OnOverlapEnd(class UPrimitiveComponent* OverlappedComp, 
	class AActor* OtherActor, 
	class UPrimitiveComponent* OtherComp, 
	int32 OtherBodyIndex) {


}

// Called every frame
void ABoids::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	if (faceCamera)
		RotateToCamera();

}

