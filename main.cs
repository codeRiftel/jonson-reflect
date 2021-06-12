using System;
using option;
using vjp;
using vjp.reflect;
using System.Collections;
using System.Collections.Generic;

class Init {
    public struct FieldBoolTest {
        public bool b;
    }

    public struct FieldIntTest {
        public int x;
    }

    public struct FieldFloatTest {
        public float x;
    }

    public struct FieldStringTest {
        public string s;
    }

    public class FieldNullTest {
        public object field;
    }

    public struct FloatArrayTest {
        public float[] floats;
    }

    public enum God {
        Hades,
        Zeus,
        Athena,
        Artemis
    }

    private static void GenerateTest() {
        FieldBoolTest boolTest = new FieldBoolTest();
        boolTest.b = true;

        FieldIntTest intTest = new FieldIntTest();
        intTest.x = -5;

        FieldFloatTest floatTest = new FieldFloatTest();
        floatTest.x = 42.54f;

        FieldStringTest stringTest = new FieldStringTest();
        stringTest.s = "hello, world";

        FieldNullTest fieldNullTest = new FieldNullTest();
        fieldNullTest.field = null;

        FloatArrayTest floatArrTest = new FloatArrayTest();
        floatArrTest.floats = new float[] { -1.5f, 0f, 10f, 42.5f };

        Dictionary<string, int> dictTest = new Dictionary<string, int>();
        dictTest["1"] = 1;
        dictTest["2"] = 2;

        Dictionary<string, List<float>> compound;
        compound = new Dictionary<string, List<float>>();
        compound["1"] = new List<float>();
        compound["1"].Add(-1.19f);
        compound["1"].Add(-1.18f);
        compound["2"] = new List<float>();
        compound["2"].Add(2.19f);
        compound["2"].Add(2.2f);


        List<object> tests = new List<object>();
        tests.Add(boolTest);
        tests.Add(intTest);
        tests.Add(floatTest);
        tests.Add(stringTest);
        tests.Add(fieldNullTest);
        tests.Add(null);
        tests.Add(10);
        tests.Add(false);
        tests.Add(floatArrTest);
        tests.Add(new string[] { "hello", ",", "world" });
        tests.Add(dictTest);
        tests.Add(compound);
        tests.Add('t');
        tests.Add(God.Zeus);

        for (int i = 0; i < tests.Count; i++) {
            object test = tests[i];
            JSONType jsonType = Reflect.ToJSON(test, true);
            Console.WriteLine(VJP.Generate(jsonType));
        }
    }

    private static void FillTest() {
        Dictionary<string, int> dictInt = new Dictionary<string, int>();
        Result<JSONType, JSONError> jsonRes = VJP.Parse("{\"1\": 1, \"2\": 2}", 1024);
        if (jsonRes.IsOk()) {
            dictInt = Reflect.FromJSON(dictInt, jsonRes.AsOk());
            Console.WriteLine(VJP.Generate(Reflect.ToJSON(dictInt, false)));
        }

        Dictionary<string, string> dictStr = new Dictionary<string, string>();
        jsonRes = VJP.Parse("{\"1\": \"1\", \"2\": \"2\"}", 1024);
        if (jsonRes.IsOk()) {
            dictStr = Reflect.FromJSON(dictStr, jsonRes.AsOk());
            Console.WriteLine(VJP.Generate(Reflect.ToJSON(dictStr, false)));
        }

        string[] strArr = new string[] { };
        jsonRes = VJP.Parse("[\"hello\",\",\",\"world\"]", 1024);
        if (jsonRes.IsOk()) {
            strArr = Reflect.FromJSON(strArr, jsonRes.AsOk());
            Console.WriteLine(VJP.Generate(Reflect.ToJSON(strArr, false)));
        }

        List<string> strList = new List<string>();
        jsonRes = VJP.Parse("[\"hello\",\",\",\"world\"]", 1024);
        if (jsonRes.IsOk()) {
            strList = Reflect.FromJSON(strList, jsonRes.AsOk());
            Console.WriteLine(VJP.Generate(Reflect.ToJSON(strList, false)));
        }

        FieldBoolTest[] fieldBoolArr = new FieldBoolTest[] { };
        jsonRes = VJP.Parse("[{\"b\": true}, {\"b\": false}]", 1024);
        if (jsonRes.IsOk()) {
            fieldBoolArr = Reflect.FromJSON(fieldBoolArr, jsonRes.AsOk());
            Console.WriteLine(VJP.Generate(Reflect.ToJSON(fieldBoolArr, false)));
        }

        Dictionary<string, List<float>> compound = new Dictionary<string, List<float>>();
        jsonRes = VJP.Parse("{\"first\": [-42.42, 10.2], \"second\": [-10.2, 42.42]}", 1024);
        if (jsonRes.IsOk()) {
            compound = Reflect.FromJSON(compound, jsonRes.AsOk());
            Console.WriteLine(VJP.Generate(Reflect.ToJSON(compound, false)));
        }

        List<God> enumList = new List<God>();
        jsonRes = VJP.Parse("[3, 2, 1]", 1024);
        if (jsonRes.IsOk()) {
            enumList = Reflect.FromJSON(enumList, jsonRes.AsOk());
            Console.WriteLine(enumList[0] == God.Artemis);
            Console.WriteLine(enumList[1] == God.Athena);
            Console.WriteLine(enumList[2] == God.Zeus);
            Console.WriteLine(VJP.Generate(Reflect.ToJSON(enumList, false)));
        }
    }

    private static int Main(string[] args) {
        GenerateTest();
        FillTest();
        return 0;
    }
}
