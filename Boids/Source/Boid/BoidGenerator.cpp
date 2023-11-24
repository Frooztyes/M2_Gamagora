// Fill out your copyright notice in the Description page of Project Settings.


#include "BoidGenerator.h"
#include <format>
#include <Components/SphereComponent.h>
#include "ElectricalBoid.h"

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

TArray<ABoids *> ABoidGenerator::Initialize(
	UMaterial* matLoc, int numBoids, UStaticMesh* staticMesh, FVector offsetPlayer, UNiagaraSystem* nsParticle,
	float moveCloserS, 
	float distanceToNeigh, 
	float catchupVelocity,
	float attractionForce,
	float _vLim)
{
	this->mat = matLoc;
	this->NumBoids = numBoids;
	this->offset = offsetPlayer;
	this->boidMesh = staticMesh;

	moveCloserStrength = moveCloserS;
	moveAwayDistance = distanceToNeigh;
	moveWithStrength = catchupVelocity;
	tendToPlaceStrength = attractionForce;
	particle = nsParticle;
	boundToPosition = false;

	vLim = _vLim;

	GenerateBoids();
	return boids;
}

ABoids * ABoidGenerator::AddBoid() {
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
	boid->Initialize(mat, boidMesh, true, false, this);
	boids.Add(boid);
	return boid;
}

void ABoidGenerator::GenerateBoids() {
	for (int i = 0; i < NumBoids - 1; i++)
	{
		AddBoid();
	}
}

void ABoidGenerator::Launch(FVector position) {
	this->launchPosition = position;
	this->isLaunched = true;
	vLim = vLim * 3;
}

FVector ABoidGenerator::MoveCloser(ABoids* currentBoid) {
	FVector pc(0, 0, 0);
	if (moveCloserStrength <= 0) return pc;

	for (auto* b : boids)
		if (currentBoid->GetActorLocation() != b->GetActorLocation())
			pc += b->GetActorLocation();
	
	pc /= boids.Num() - 1;

	return (pc - currentBoid->GetActorLocation()) / moveCloserStrength;
}

FVector ABoidGenerator::MoveWith(ABoids* currentBoid) {
	FVector pv(0, 0, 0);
	if (moveWithStrength <= 0) return pv;

	for (auto* b : boids)
		if (currentBoid->GetActorLocation() != b->GetActorLocation())
			pv += b->Velocity;

	pv /= boids.Num() - 1;

	return (pv - currentBoid->Velocity) / moveWithStrength;
}

FVector ABoidGenerator::MoveAway(ABoids* currentBoid) {
	FVector c(0, 0, 0);
	FVector bjPosition = currentBoid->GetActorLocation();
	for (auto* b : boids) {
		FVector bPosition = b->GetActorLocation();
		if (currentBoid->GetActorLocation() != b->GetActorLocation()) {
			if ((bPosition - bjPosition).Length() < moveAwayDistance) {
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
	if (currentBoid->Velocity.Length() > vLim) {
		currentBoid->Velocity = (currentBoid->Velocity / currentBoid->Velocity.Length()) * vLim;
	}
}

void ABoidGenerator::MoveBoids(float DeltaTime) {
	for (auto* boid : boids) {
		if (!boid) {
			boids.Remove(boid);
			continue;
		}

		boid->Velocity += MoveWith(boid); // rule 1
		boid->Velocity += MoveAway(boid); // rule 2
		boid->Velocity += MoveCloser(boid); // rule 3

		//if(boundToPosition)
		//	boid->Velocity += BoundPosition(boid);
		if(tendToPosition)
			boid->Velocity += TendToPlace(boid);

		if(limitVelocity)
			LimitVelocity(boid);

		/*if (isLaunched) {
			boid->Velocity *= acceleration;
			acceleration += 0.05f;
		}*/

		boid->SetActorLocation(boid->GetActorLocation() + boid->Velocity * DeltaTime * 30);
	}
}

int ABoidGenerator::GetBoidsNumber() {
	return boids.Num();
}

// Called every frame
void ABoidGenerator::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	MoveBoids(DeltaTime);
	if (isLaunched) {
		//SetActorLocation(FMath::Lerp(GetActorLocation(), launchPosition, DeltaTime));
		SetActorLocation(launchPosition);
	}
	else {
		SetActorLocation(player->GetActorLocation() + offset);
	}
}

