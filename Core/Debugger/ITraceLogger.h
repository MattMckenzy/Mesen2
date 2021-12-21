#pragma once
#include "stdafx.h"
#include "DebugTypes.h"

struct TraceRow
{
	uint32_t ProgramCounter;
	CpuType Type;
	uint8_t ByteCode[4];
	uint8_t ByteCodeSize;
	char LogOutput[500];
};

struct TraceLoggerOptions
{
	bool Enabled;
	bool IndentCode;
	bool UseLabels;
	char Condition[1000];
	char Format[1000];
};

class ITraceLogger
{
protected:
	bool _enabled = false;

public:
	static uint64_t NextRowId;

	virtual int64_t GetRowId(uint32_t offset) = 0;
	virtual void GetExecutionTrace(TraceRow& row, uint32_t offset) = 0;
	virtual void SetOptions(TraceLoggerOptions options) = 0;

	__forceinline bool IsEnabled() { return _enabled; }
};