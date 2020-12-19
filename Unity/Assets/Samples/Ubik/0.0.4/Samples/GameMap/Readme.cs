/*
1. 当前GameMap文件夹包含原始的地图，
    所有的模型都在materials里
    所有的prefab都在prefabs里，
    所有的代码都在scripts里，
    将GameMap.unity添加到File->building settings里面就能运行

2. 之后再添加可以将模型放到materials文件夹，
    prefab放到prefabs文件夹（spawn的时候将prefab添加到prefabs->Prefab Catalogue）

3. 功能：
    1. dice.cs：当avatar拿起骰子时，记录avatar_id, avatar丢出骰子后，记录结果。并发送Message{avatar_id, result}。
    2. Avatar_list.cs: 获取玩家id、position等信息（仅测试）
    3. GraspableObject.cs: 设置物体可抓取
    4. GameController.cs: 接收dice的信息，控制玩家顺序
    5. Mapblocks.cs: 接收GameController.cs信息，用于判断是否触发事件
    6. 。。。。。。
*/