// Fill out your copyright notice in the Description page of Project Settings.


#include "BoidGenerator.h"
#include <format>
#include <Components/SphereComponent.h>

// Sets default values
ABoidGenerator::ABoidGenerator()
{
	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	VisualMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Mesh"));
	VisualMesh->SetupAttachment(RootComponent);
	
	static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeVisualAsset(TEXT("/Game/StarterContent/Shapes/Shape_Cube.Shape_Cube"));

	if (CubeVisualAsset.Succeeded())
	{
		VisualMesh->SetStaticMesh(CubeVisualAsset.Object);
		VisualMesh->SetRelativeLocation(FVector(0.0f, 0.0f, 0.0f));
		VisualMesh->SetCollisionProfileName(TEXT("NoCollision"));
	}
}

// Called when the game starts or when spawned
void ABoidGenerator::BeginPlay()
{
	Super::BeginPlay();

	VisualMesh->SetStaticMesh(mesh);

	using namespace std::chrono;
	milliseconds ms = duration_cast<milliseconds>(
		system_clock::now().time_since_epoch()
	);
	FMath::SRandInit(ms.count());
	FMath::RandInit(ms.count());

	player = GetWorld()->GetFirstPlayerController()->GetPawn();
}

void ABoidGenerator::Initialize(UMaterial* matLoc, int numBoids, UStaticMesh* staticMesh, FVector offsetPlayer) {
	this->mat = matLoc;
	this->NumBoids = numBoids;
	this->offset = offsetPlayer;
	this->boidMesh = staticMesh;
	GenerateBoids();
}

void ABoidGenerator::AddBoid() {
	FRotator SpawnRotation(90, 0, 0);

	using namespace std::chrono;
	milliseconds ms = duration_cast<milliseconds>(
		system_clock::now().time_since_epoch()
	);
	FMath::SRandInit(ms.count());
	FMath::RandInit(ms.count() + boids.Num() * ms.count());

	FVector SpawnLocation(
		FMath::RandRange(-radiusBorder, radiusBorder),
		FMath::RandRange(-radiusBorder, radiusBorder),
		FMath::RandRange(-radiusBorder, radiusBorder)
	);

	ABoids* boid = GetWorld()->SpawnActor<ABoids>(GetActorLocation() + SpawnLocation, SpawnRotation);
	boid->Initialize(mat, boidMesh, true, false);
	//boid->AttachToActor(this, FAttachmentTransformRules::KeepWorldTransform);
	//boid->SetActorLocation(GetActorLocation() + SpawnLocation);

	boids.Add(boid);
}

void ABoidGenerator::GenerateBoids() {
	for (int i = 0; i < NumBoids; i++)
	{
		AddBoid();
	}
}

void ABoidGenerator::Launch(FVector position) {
	this->launchPosition = position;
	this->isLaunched = true;
}

FVector ABoidGenerator::MoveCloser(ABoids* currentBoid) {
	FVector pc(0, 0, 0);
	if (moveCloserStrength <= 0) return pc;

	for (auto* b : boids)
		if (currentBoid != b)
			pc += b->GetActorLocation();
	
	pc /= boids.Num() - 1;

	return (pc - currentBoid->GetActorLocation()) / moveCloserStrength;
}

FVector ABoidGenerator::MoveWith(ABoids* currentBoid) {
	FVector pv(0, 0, 0);
	if (moveWithStrength <= 0) return pv;

	for (auto* b : boids)
		if (currentBoid != b)
			pv += b->Velocity;

	pv /= boids.Num() - 1;

	return (pv - currentBoid->Velocity) / moveWithStrength;
}

FVector ABoidGenerator::MoveAway(ABoids* currentBoid) {
	FVector c(0, 0, 0);
	FVector bjPosition = currentBoid->GetActorLocation();
	for (auto* b : boids) {
		FVector bPosition = b->GetActorLocation();
		if (currentBoid != b) {
			if (FVector::Dist(bPosition, bjPosition) < moveAwayDistance) {
				c -= (bPosition - bjPosition);
			}
		}
	}
	return c;
}


bool ABoidGenerator::IsInSphere(FVector center, FVector point, float radius) {
	return (center.X - point.X) * (center.X - point.X) +
		(center.Y - point.Y) * (center.Y - point.Y) +
		(center.Z - point.Z) * (center.Z - point.Z) - radius * radius <= 0;
}

FVector ABoidGenerator::TendToPlace(ABoids* currentBoid) {
	FVector place = GetActorLocation();
	return (place - currentBoid->GetActorLocation()) / tendToPlaceStrength;
}

FVector ABoidGenerator::BoundPosition(ABoids* currentBoid) {
	FVector actorLocation = GetActorLocation();
	FVector boidLoc = currentBoid->GetActorLocation();
	FVector v(0, 0, 0);
	//if (!IsInSphere(actorLocation, boidLoc, radiusBorder)) {
	//	FVector actorToBoid = (actorLocation - boidLoc);
	//	float norm = actorToBoid.Length();
	//	currentBoid->Velocity += actorToBoid * actorToBoid.Length() * actorToBoid.Length() / bounceCoef;
	//	//boid->Velocity += actorToBoid * FMath::SRand() / bounceCoef;
	//}

	if (boidLoc.X < actorLocation.X - radiusBorder) {
		v.X = 10;
	} else if (boidLoc.X > actorLocation.X + radiusBorder) {
		v.X = -10;
	}

	if (boidLoc.Y < actorLocation.Y - radiusBorder) {
		v.Y = 10;
	}
	else if (boidLoc.Y > actorLocation.Y + radiusBorder) {
		v.Y = -10;
	}

	if (boidLoc.Z < actorLocation.Z - radiusBorder) {
		v.Z = 10;
	}
	else if (boidLoc.Z > actorLocation.Z + radiusBorder) {
		v.Z = -10;
	}
	return v;
}

void ABoidGenerator::LimitVelocity(ABoids * currentBoid) {
	float vLim = 10;
	if (currentBoid->Velocity.Length() > vLim) {
		currentBoid->Velocity = (currentBoid->Velocity / currentBoid->Velocity.Length()) * vLim;
	}
}

void ABoidGenerator::MoveBoids() {
	for (auto* boid : boids) {
		boid->Velocity += MoveWith(boid);
		boid->Velocity += MoveCloser(boid);
		boid->Velocity += MoveAway(boid);
		if(boundToPosition)
			boid->Velocity += BoundPosition(boid);
		if(tendToPosition)
			boid->Velocity += TendToPlace(boid);
		if(limitVelocity)
			LimitVelocity(boid);
		if (isLaunched) {
			boid->Velocity *= acceleration;
			acceleration += 0.05f;
		}

		boid->SetActorLocation(boid->GetActorLocation() + boid->Velocity);
	}
}

// Called every frame
void ABoidGenerator::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	MoveBoids();
	if (isLaunched) {
		//SetActorLocation(FMath::Lerp(GetActorLocation(), launchPosition, DeltaTime));
		SetActorLocation(launchPosition);
	}
	else {
		SetActorLocation(player->GetActorLocation() + offset);
	}
}

