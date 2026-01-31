using System;
using CodeExecutor;
using GlobalSettings;
using UnityEngine;

public class Test : MonoBehaviour
{
    private LuaVM luaVM;
    private double lastFrameCount = GlobalInfos.Instance.MelodiaCount;
    void Start()
    {
        luaVM = new LuaVM(new LuaVMConfigurer(8,
            vm => {
                LuaVMAdaptorLib.AtomicCAS(vm); LuaVMAdaptorLib.CheckThread(vm);
                LuaVMAdaptorLib.GetCurrentThreadID(vm); LuaVMAdaptorLib.HangupCurrentThread(vm);
                LuaVMAdaptorLib.NewThread(vm); LuaVMAdaptorLib.GetCurrentFrameCount(vm);
                LuaVMAdaptorLib.GetPosition(vm);
                LuaVMAdaptorLib.Move(vm); LuaVMAdaptorLib.UseItem(vm);
                LuaVMAdaptorLib.CanHarvest(vm); LuaVMAdaptorLib.Harvest(vm);
                LuaVMAdaptorLib.TrySetFacility(vm); LuaVMAdaptorLib.InteractWith(vm);
                vm.GetLuaVM().Options.DebugPrint = Debug.Log;
            }, vm => { }, LuaVMAdaptorLib.CheckCurrentBotIsBusy), 
            "TestScript", @"
local safe_table = {}

-- 辅助函数：交换表中两个位置的值
local function swap(t, i, j)
    local temp = t[i]
    t[i] = t[j]
    t[j] = temp
end

-- 辅助函数：分区函数（快速排序用）
local function partition(t, low, high, comp)
    local pivot = t[high]
    local i = low - 1
    
    for j = low, high - 1 do
        if comp(t[j], pivot) then
            i = i + 1
            swap(t, i, j)
        end
    end
    
    swap(t, i + 1, high)
    return i + 1
end

-- 辅助函数：快速排序实现
local function quicksort(t, low, high, comp)
    if low < high then
        local pi = partition(t, low, high, comp)
        quicksort(t, low, pi - 1, comp)
        quicksort(t, pi + 1, high, comp)
    end
end

-- 辅助函数：堆排序实现（可选，更稳定）
local function heapsort(t, comp)
    local n = #t
    
    -- 构建最大堆
    local function heapify(i, size)
        local largest = i
        local left = 2 * i
        local right = 2 * i + 1
        
        if left <= size and comp(t[largest], t[left]) then
            largest = left
        end
        
        if right <= size and comp(t[largest], t[right]) then
            largest = right
        end
        
        if largest ~= i then
            swap(t, i, largest)
            heapify(largest, size)
        end
    end
    
    -- 构建初始堆
    for i = math.floor(n / 2), 1, -1 do
        heapify(i, n)
    end
    
    -- 一个个提取元素
    for i = n, 2, -1 do
        swap(t, 1, i)
        heapify(1, i - 1)
    end
end

-- 实现 table.sort
function safe_table.sort(t, comp)
    -- 参数检查
    if type(t) ~= ""table"" then
        error(""bad argument #1 to 'sort' (table expected, got "" .. type(t) .. "")"", 2)
    end
    
    -- 默认比较函数
    if comp == nil then
        comp = function(a, b)
            return a < b
        end
    elseif type(comp) ~= ""function"" then
        error(""bad argument #2 to 'sort' (function expected, got "" .. type(comp) .. "")"", 2)
    end
    
    local n = #t
    if n <= 1 then
        return
    end
    
    -- 检查数组部分是否连续
    local is_array = true
    for i = 1, n do
        if t[i] == nil then
            is_array = false
            break
        end
    end
    
    if not is_array then
        error(""attempt to sort a table with nil values"", 2)
    end
    
    -- 使用快速排序（Lua 原生实现使用快速排序）
    -- quicksort(t, 1, n, comp)
    
    -- 或者使用堆排序（更稳定，O(n log n)最坏情况）
    heapsort(t, comp)
end


function for_all(f)
	local function row()
		for i=1, 32 do
			f()
			move(1)
		end
    end

	for i=1, 32 do
		if not new_thread(row) then row() end
		move(2)
    end
end

function goto_pos(x, y)
    local now_x, now_y = get_x_pos(), get_y_pos()
    while now_x ~= x do move(1) now_x = get_x_pos() end
    while now_y ~= y do move(2) now_y = get_y_pos() end
end

function set()
    build(2)
    build(4)
    build(32)
    build(64)
end

info_t = {}
function collect_info()
    local x, y = get_x_pos(), get_y_pos()
    local pos = x + y * 32 + 1
    info_t[pos] = interact_with('get_edges')
end

for_all(set)
for_all(collect_info)

dsu_table = {}
for i=1, 1024 do dsu_table[i] = i end
function find_root(x)
    if dsu_table[x] == x then 
        return x
    else 
        dsu_table[x] = find_root(dsu_table[x])
        return dsu_table[x]
    end
end
function merge(x, y)
    local x = find_root(x)
    local y = find_root(y)
    dsu_table[x] = y
end

task_t = {}
function kruskal()
    for i=1, #edges do
        local xr = find_root(edges[i].u)
        local yr = find_root(edges[i].v);
        if xr ~= yr then
            merge(xr, yr);
            table.insert(task_t, {from = edges[i].u, to = edges[i].v})
        end
    end
end

print('test 0')
edges = {}
visited = {}
for id, targets in ipairs(info_t) do
    for _, target in ipairs(targets) do
        if visited[{id, target.x + target.y*32 + 1}] or visited[{target.x + target.y*32 + 1, id}] then goto continue end;
        visited[{id, target.x + target.y*32 + 1}] = true
        table.insert(edges, {u = id, v = target.x + target.y*32 + 1, w = target.weight})
        ::continue::
    end
end

print('test 1')
print(#edges)
safe_table.sort(edges, function(a, b) return a.w < b.w end)
print('test 2')
kruskal()
print('test 3')

function create_spin_lock() return { locked = 0 } end
function spin_lock(lock) while atomic_compare_and_swap_at(lock, 'locked', 0, 1) ~= 0 do hangup_current_thread() end end
function spin_unlock(lock) atomic_compare_and_swap_at(lock, 'locked', 1, 0) end

task_id_lock = create_spin_lock()
task_id = 1
function link_nodes()
    while true do
        spin_lock(task_id_lock)
        if not task_t[task_id] then return end
        local task = task_t[task_id]
        task_id = task_id + 1
        spin_unlock(task_id_lock)

        goto_pos((task.from-1) % 32, math.floor((task.from-1) / 32))
        if not interact_with('connect', (task.to-1) % 32, math.floor((task.to-1) / 32)) then error('WTF??') end
    end
end

for_all(link_nodes)
for i = 1, 500 do hangup_current_thread() end
harvest()

            ");
    }

    private bool printed = false;
    void Update()
    {
        if (luaVM.CouldResume())
            luaVM.ResumeUntilLimit(LuaVMConfigurer.MaxInstructionPerResume * 10);
        else if (!printed)
        {
            printed = true;
            Debug.Log(luaVM.State);
            if (luaVM.State == RunningState.Faulted) Debug.Log(luaVM.ExceptionWhat);
        }
        else
        {
            ;   // trap here
        }
    }

    void OnDestroy()
    {
        luaVM?.Dispose();
    }
}
