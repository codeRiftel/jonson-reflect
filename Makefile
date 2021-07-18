.PHONY: build

build:
	mcs main.cs Jonson.cs option/*cs Reflect.cs -langversion:ISO-2 -out:ref.exe
