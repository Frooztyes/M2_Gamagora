// Fill out your copyright notice in the Description page of Project Settings.


#include "ElectricalBoid.h"
//#include <NiagaraFunctionLibrary.h>
#include "NiagaraFunctionLibrary.h"
#include "NiagaraComponent.h"

void AElectricalBoid::Initialize(UMaterial* mat, UStaticMesh* mesh, UNiagaraSystem* nsParticle, bool faceCam, bool debugCircle, ABoidGenerator* boidGenerator)
{
	//Super::Initialize(mat, mesh, false, false, boidGenerator);
	//particle = nsParticle;

	//UNiagaraComponent* NiagaraCom = UNiagaraFunctionLibrary::SpawnSystemAttached(
	//	particle,
	//	VisualMesh,
	//	NAME_None,
	//	FVector(0.f, 0.f, VisualMesh->Bounds.BoxExtent.Z),
	//	FRotator(0.f),
	//	EAttachLocation::Type::SnapToTarget,
	//	true
	//);
}

void AElectricalBoid::BeginPlay()
{
	Super::BeginPlay();
}
