﻿#pragma once

namespace WatsonRegistrationUtility
{
	public ref class WatsonRegistrationManager sealed
	{
	public:
		WatsonRegistrationManager();
		static void Start(Platform::String^ appSecret);
		static void SetCorrelationId(Platform::String^ correlationId);
	};
}
