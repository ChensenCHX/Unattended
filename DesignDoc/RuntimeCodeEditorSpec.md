# 运行时代码编辑器规范（轻量 Lua 编辑器）

## 概要 ✅
本文件记录针对游戏内（runtime）轻量代码编辑器的已确认设计与实现契约。编辑器面向玩家用于在游戏内编写/编辑 Lua 脚本（编程游戏场景）。

核心要点：
- 目标平台：Unity 2022.3 LTS，Standalone(PC)
- 编辑器类型：轻量可编辑器（实时语义着色、撤销/重做、行号、行号断点）
- 语义着色：本地轻量解析，区分基础 token 并区分 `local` / `global`
- 用户输入：使用原生输入框，确保 IME (中文) 正常工作

---

## 已确认的设计决策（来自讨论） 🎯
- 语义着色实现：轻量本地解析（无原生插件）。着色类别：keyword / string / number / comment / builtin / local / global / function。
- 缩进：真实 Tab（按键插入 Tab 字符），支持自动缩进（按 Enter 复制/调整缩进层级）。
- 换行/滚动：不自动换行；编辑器会根据内容宽度自动扩展并支持横向滚动；不截断长行。
- 断点行为：点击行号切换断点；在行号左侧显示红点；断点持久化到磁盘以便恢复；提供断点切换事件。
- 行命中回调：支持外部调用以“命中并高亮某行”（用于运行时调试反馈）。
- 保存策略：编辑器**不直接写文件**；通过事件将保存请求交给宿主系统处理（你实现的脚本 VM/IO）。
- 性能：解析与着色在后台线程/任务中完成，使用防抖（debounce）策略，目标交互延迟 ≲ 200 ms，且不阻塞主线程。
- 可扩展性：单文件不设硬上限；但超大文件将降级为仅文本模式（无语义着色）并通过 UI 提示。
- 可访问性/样式：暗色（VSCode 风格），等宽字体（非连字），不显示当前行高亮/缩进线/不可见字符，默认字体大小使用 Unity 输入框的默认值。

---

## 功能规格（验收准则） ✅
- 基本编辑：插入/删除、撤销/重做、真实 Tab、自动缩进。
- 语法着色：对常见 Lua 源码进行语义级着色（按决策的类别），在 200 ms 内更新（常规文件）。
- 行号与断点：行号展示、点击切换断点、断点在 gutter 显示红点、断点在会话间持久化并能在加载时恢复。
- 行命中高亮：外部可调用接口通知某行命中，编辑器在 UI 上短时高亮该行并可选滚动到视图内。
- IME 友好：中文输入法在任意编辑位置均工作正常，输入过程中语法高亮不造成光标错位或干扰。
- API：通过事件/方法与宿主交互（详见下节）。

---

## 对外 API（运行时 C#）🔗
事件（UnityEvents / C# events）：
- `event Action<string /*path*/, string /*text*/> OnContentChanged` — 内容变更（防抖后触发）。
- `event Action<string /*path*/, int /*line*/, bool /*enabled*/> OnBreakpointToggled` — 断点启/禁。

公共方法：
- `void LoadText(string path, string text)` — 加载文本到编辑器。
- `string GetText(string path)` — 获取指定文件的当前文本。
- `void BreakpointMatchLine(string path, int line, float duration = 1f)` — 高亮指定行（用于 "行命中" 提示）。
- `void RunningAtLine(string path, int line, float duration = 1f)` — 高亮指定行（用于 "正在执行该行" 提示）。
- `List<int> GetBreakpoints(string path)` — 返回指定文件的断点列表。
- `void RestoreBreakpoints(string path, List<int> breakpoints)` — 恢复断点（由宿主提供持久化数据）。

事件与持久化约定：
- 编辑器负责在本地内存维护当前断点集合并在变化时触发 `OnBreakpointToggled`。
- 持久化由宿主决定：编辑器在断点变更后触发 `OnBreakpointToggled`，宿主应调用 `RestoreBreakpoints` 在加载时恢复。

---

## UI / 视觉（暗色 —— VSCode 风格） 🎨
- 主体颜色：深灰背景 (#1e1e1e 或接近)；文本基色偏浅灰/白色；关键字带蓝/紫；字符串绿色；注释灰绿色/偏暗色。
- 字体：等宽（系统等宽或推荐 `Fira Code` / `JetBrains Mono`，但不启用连字）。
- 布局：
  - 左侧为行号 gutter（含断点层，宽度自适应数字长度）。
  - 右侧为正文编辑区（可横向滚动）。
  - 顶部/底部保留小面积用于状态（行/列、语言、保存状态等）。
- 无当前行高亮、无缩进线、不可见字符不显示。

---

## 实现/工程建议（技术细节） 🔧
- 解析器：实现一个轻量 Lua 词法分析器 + 简单作用域追踪以识别 `local` 与非 `local` 标识符（无需完整 AST）。
- 着色：增量/行级着色；对大文件按区块分帧处理；使用防抖（例如 150–250 ms）触发解析。
- 线程模型：解析在后台 Task/线程执行；结果通过主线程应用（Unity 主线程）。
- 编辑器控件：使用 UI Toolkit 的 `TextField` / `TextElement` 或自定义 `TextInput` 组件作为基础（保持原生输入事件以支持 IME）。
- 断点持久化：提供断点导出/导入 JSON 格式（host 负责实际写盘）。示例键：`editor_breakpoints/<path-hash>.json`。
- 资源与依赖：尽量不引入外部库；如果未来需要更准确的解析，可选接入 tree-sitter / MoonSharp 分析器。

---

## 窗口行为与运行时控制（Window chrome & runtime controls）
添加可拖动/可缩放的“世界空间”窗口容器，以及顶栏运行控件以支持游戏内调试与外部 IDE 跟踪。以下条目反映了已确认的行为（包含你最近的决定），并补充实现细节、持久化约定与验收测试。

### 已确认行为
- 窗口为浮动可拖拽面板（非全屏），支持多窗口并允许拖出游戏视口外。窗口在世界坐标中锚定：当玩家摄像机移动时，窗口保持相对的“世界位置”（因此相同世界坐标在屏幕上显示的位置保持一致）；摄像机的 y 值会影响窗口显示缩放（见下文缩放策略）。
- 顶栏包含按钮（默认从左到右）：
  - **开始/恢复执行**（考虑断点）：触发 `OnRunRequested`，按钮在运行/暂停状态间切换。
  - **单步执行**：触发 `OnStepRequested()`。
  - **切换代码源（External Source）**：开启后编辑器进入只读并监听对应磁盘文件的变化；开启/关闭触发 `OnExternalSourceToggled`。外部源开启时**无条件以外部文件覆盖内部文本**；外部源关闭时**无条件以内部编辑器内容覆盖外部文件**。
  - **最小化**：折叠编辑区，仅显示顶栏（保留状态指示与按键）。
  - **关闭窗口（删除文件）**：触发 `OnWindowCloseRequested`；编辑器会在文件名追加 UUID 并将文件移动到垃圾站文件夹（由宿主执行），同时从 UI 中移除窗口。
- 窗口支持鼠标拖动与防止窗口重叠策略；窗口支持 z-order 管理（点击激活置顶）。

### 世界坐标与缩放策略（实现建议，需确认）
- 窗口以一个 invisible Anchor GameObject（存储世界坐标）作为锚点，UI 使用 World-space Panel/UIDocument 渲染。
- 缩放（默认实现）：
  - 对于透视相机：scale = clamp(baseScale * (referenceDistance / distanceToCamera), minScale, maxScale)
  - 对于正交相机：scale = baseScale * (referenceOrthoSize / camera.orthographicSize)
  - 默认参数建议：baseScale = 1.0, referenceDistance = 10, minScale = 0.5, maxScale = 2.0。
- 持久化：保存 Anchor 的世界坐标（Vector3）、窗口本地尺寸（宽/高）、以及 referenceCameraId（用于在多相机场景中恢复）。

### External-Source
- 实现细节：在 Windows Standalone 使用 `FileSystemWatcher` 监听指定路径的变更并通过防抖（默认 300 ms）向编辑器推送 `OnExternalFileChanged(path)` 事件；接收到事件时**立即（无提示）在编辑器中用外部内容覆盖当前显示**
- 反向同步：当 External-Source 被关闭时，编辑器将无条件将当前文本通过 `OnEditorFileChanged(path, text)` 提交给宿主以写回磁盘。

### 删除（垃圾站）行为
- 删除时编辑器将请求宿主将文件移动到垃圾站目录并在文件名后追加 UUID（格式建议： `<originalName>__<yyyyMMddHHmmss>__<8hex>.lua`）。
- 默认垃圾站路径建议：`{Application.persistentDataPath}/runtime_code_editor_trash/`。
- 宿主负责实际的移动/恢复/永久删除操作；编辑器仅发送 `OnFileDeleteRequested(path)` 事件并在 UI 中立即移除窗口（以保证玩家体验）。

### 窗口对齐与防重叠
- 支持自动吸附（snap）到屏幕边缘、网格（默认 16 px）及其他窗口（snap 阈值默认 12 px）。
- 防重叠策略：新建或恢复窗口时按最近空闲区域放置；当窗口被移动造成重叠时在释放时自动吸附到重叠的窗口上。

### API 扩展（新增/变更，汇总）
- 事件：
  - `event Action<string /*path*/> OnRunRequested`
  - `event Action<string /*path*/> OnStepRequested`
  - `event Action<string /*path*/, bool /*enabled*/> OnExternalSourceToggled`
  - `event Action<string /*path*/> OnExternalFileChanged`
  - `event Action<string /*path*/, string /*text*/> OnEditorFileChanged`
  - `event Action<string /*path*/> OnWindowMinimized`, `OnWindowRestored`
  - `event Action<string /*path*/, string /*trashUuid*/> OnFileDeleteRequested` — 请求宿主将文件移动到垃圾站并返回 UUID。
  - `event Action<string /*path*/, Vector3 /*worldPos*/> OnWindowWorldPositionChanged`
- 方法：
  - `void SetExternalSourceMode(string path, bool watch)`
  - `void Minimize()` / `void Restore()` / `void Close(bool confirmDelete)`
  - `void ApplyExternalFileChange(string path, string newText, bool force = false)`
  - `Rect GetWindowBounds(string path)` / `void SetWindowBounds(string path, Rect bounds)`
  - `void SetWindowWorldAnchor(string path, Vector3 worldPosition)` / `Vector3 GetWindowWorldAnchor(string path)`
  - `void SaveWindowState(string path)` / `WindowState LoadWindowState(string path)`

### 验收要点（新增 & 已确认）
- 窗口在世界坐标中锚定并且可被拖出视口；当摄像机移动或缩放时窗口保持相对世界位置且按摄像机 y 值变化缩放（参照缩放策略）。
- 多窗口：可同时打开多个编辑器窗口，窗口间支持 z-order、吸附与防重叠。
- External-Source：使用 `FileSystemWatcher` 在 ≤1s 内检测外部更改并无条件覆盖内部文本；外部模式下运行/单步按钮保持可用并正常触发事件。
- 删除：删除操作会将文件移动到垃圾站并在文件名追加 UUID。

---


## 验收标准与测试用例 ✅
- 在常见 Lua 文件（≤5k 行）中，编辑—光标—输入流畅且 IME 无异常。
- 点击行号能立即在 gutter 显示/隐藏断点，并触发 `OnBreakpointToggled`。
- 断点在重新打开同一文件时被恢复（宿主通过 `RestoreBreakpoints` 恢复）。
- 外部触发 `HighlightLine(42)` 会高亮第 42 行。
- 在大文件（>50k 行）下，编辑器降级为文本模式并展示提示（无语义着色）。

---

## 交付清单（优先级排序） 📦
1. 必需：`DesignDoc/RuntimeCodeEditorSpec.md`（本文件）
2. 视觉草案：`Assets/UI/RuntimeCodeEditor/RuntimeCodeEditor.uxml` + `RuntimeCodeEditor.uss`
3. 运行时代码：`Assets/Scripts/UI/RuntimeCodeEditor/RuntimeCodeEditor.cs`
4. 辅助：`LuaSyntaxHighlighter.cs`, `BreakpointManager.cs`
5. Demo：`Assets/Scenes/RuntimeCodeEditorDemo.unity`（包含示例脚本与断点演示）
6. 单元/集成测试（编辑交互、断点事件、IME 测试用例）