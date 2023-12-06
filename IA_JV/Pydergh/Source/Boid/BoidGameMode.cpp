// Copyright Epic Games, Inc. All Rights Reserved.

#include "BoidGameMode.h"
#include "BoidCharacter.h"
#include "UObject/ConstructorHelpers.h"

ABoidGameMode::ABoidGameMode()
{
	// set default pawn class to our Blueprinted character
	static ConstructorHelpers::FClassFinder<APawn> PlayerPawnBPClass(TEXT("/Game/ThirdPerson/Blueprints/BP_ThirdPersonCharacter"));
	if (PlayerPawnBPClass.Class != NULL)
	{
		DefaultPawnClass = PlayerPawnBPClass.Class;
	}
}
