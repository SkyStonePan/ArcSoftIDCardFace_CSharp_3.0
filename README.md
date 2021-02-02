快速上手：

	1.安装 VS2013 环境安装包（vcredist_x86_vs2013.exe） 
	2. 从官网（https://ai.arcsoft.com.cn）申请人证 SDK C/C++ 3.0 版本 ，下载对应的 sdk 版 本(x86 或 x64)，并解压 
	3. 将 libs 中的 SDK 库文件（libarcsoft_face.dll、libarcsoft_face_engine.dll、 libarcsoft_idcardveri.dll）拷贝到工程 bin 目录的对应平台的 debug 或 release 目录下 
	4. 将对应 appid、appkey 和 activekey 替换 App.config 文件中对应内容 
	5. 在 Debug 或者 Release 中选择配置管理器，选择对应的平台 
	6. 连接摄像头，并确认摄像头能正常工作 
	7. 程序中内置图片，作为身份证照片，如果需要其他照片作为身份证照片，直接将图 片文件（仅支持.png、.jpg、.bmp）放置到运行目录即可（运行过程中可以直接放到 运行目录（命名尽量靠前），点击“读取身份证”按钮即可更新模拟读取的身份证图 片）。 
	8. 按 F5 启动程序 


常见问题：

	1.后引擎初始化失败	
		(1)请选择对应的平台，如x64,x86 
		(2)删除bin下面对应的idv_install.dat；
		(3)请确保App.config下的appid、appkey 和 activekey与当前sdk一一对应。


​		
	2.使用人脸检测功能对图片大小有要求吗？	
		推荐的图片大小最大不要超过2M，因为图片过大会使人脸检测的效率不理想，当然图片也不宜过小，否则会导致无法检测到人脸。
		
	3.使用人脸识别引擎提取到的人脸特征信息是什么？	
		人脸特征信息是从图片中的人脸上提取的人脸特征点，是byte[]数组格式。 
		
	4.SDK人脸比对的阈值设为多少合适？	
		推荐值为0.82，用户可根据不同场景适当调整阈值。
		
	5.可不可以将人脸特征信息保存起来，等需要进行人脸比对的时候直接拿保存好的人脸特征进行比对？
		可以，当人脸个数比较多时推荐先存储起来，在使用时直接进行比对，这样可以大大提高比对效率。存入数据库时，请以Blob的格式进行存储，不能以string或其他格式存储。