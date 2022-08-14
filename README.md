# TEditor
![](https://user-images.githubusercontent.com/8959735/146024526-a4ebc5a0-9605-4997-af06-554ee27618b9.png)  
欢迎使用TEditor，具有图形化编辑器的模板图片批量生成器。  

# 主要功能
- 图形化界面，支持文字、图片、基础图形
- 支持从Excel、csv导入数据
- 文字和图片地址均可以由数据计算而来
- 使用图层概念，支持剪贴蒙版用于制作异形图片
- 支持条件格式组，针对数据设置不同条件，可以显示/隐藏不同元素
- 支持PS快捷键，用起来还是那么熟悉
- 更多……

# 如何使用
## UI界面
这个人很懒，啥教程也不想写（

## 命令行
TEditor支持通过命令行调用进行批量生成，方便集成到自动化流程中  
注：因WPF程序限制，在启动命令前需要加start /wait，否则程序将立即返回，无法正确判断生成进度。无法在无GUI的系统中使用，请使用带有GUI的Windows系统。  

使用例：
```
start /wait TEditor.exe batchgen -i "模板文件.ted" -d "数据源.csv" -o "c:\output" -n "R-{index}" -s 1 -e 10
```

详情参数请使用`start /wait TEditor.exe batchgen --help`查询，参数的效果与UI中的功能一致。

# 使用协议（暂定）
个人制作周刊月刊、证卡、工牌、名片等用途可免费使用。如果本软件有帮助到你，还请通知天书一下~  
允许基于个人用途，修改本软件并再次分发，但需要保留作者信息。  
禁止商用，如需用于商业用途需要获得许可。  
欢迎参与开发，请联系天书！

# 谁在使用TEditor
- [CVSE+](https://space.bilibili.com/151439645)
- [鬼畜周刊组](https://space.bilibili.com/14498772)

# 由来
本软件由[BiliRanking](https://github.com/SkiTiSu/BiliRanking)中的模板图批量生成功能立下的flag而来，希望能够做成一个通用的、图形化的模板批量生成器，可以由使用者调整内容，而不是像BR一样需要由开发者在代码中不断修改。  
TEditor即Template/TiSu Editor，（天书的）模板编辑器，一开始只想做成BR的一个模块，后来决定成为独立软件，可以导入数据。