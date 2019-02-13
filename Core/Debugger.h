#pragma once
#include "stdafx.h"
#include "CpuTypes.h"

class Cpu;
class MemoryManager;

enum class MemoryOperationType;
class TraceLogger;
//class Disassembler;

struct DebugState
{
	CpuState Cpu;
	//PpuState ppuState;
	//ApuState apuState;
};

class Debugger
{
private:
	shared_ptr<Cpu> _cpu;
	shared_ptr<MemoryManager> _memoryManager;

	shared_ptr<TraceLogger> _traceLogger;
	//unique_ptr<Disassembler> _disassembler;

	atomic<int32_t> _cpuStepCount;

public:
	Debugger(shared_ptr<Cpu> cpu, shared_ptr<MemoryManager> memoryManager);
	~Debugger();

	void ProcessCpuRead(uint32_t addr, uint8_t value, MemoryOperationType type);
	void ProcessCpuWrite(uint32_t addr, uint8_t value, MemoryOperationType type);

	void Run();
	void Step(int32_t stepCount);
	bool IsExecutionStopped();

	shared_ptr<TraceLogger> GetTraceLogger();
};