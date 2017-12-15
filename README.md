# JsonPatch
Library to apply JSON Patches according to [RFC 6902](tools.ietf.org/html/rfc6902)

You can using it to compare two jsons, get differences in JsonPatch format.

# API Usage

* **Diff**

1. basic compare
```C#
 string left =  "{\"a\":1,\"b\":2, \"c\":3}";
 string right = "{\"a\":1,\"b\":2}"; 
 Differ differ = new Differ();
 JArray diffs = differ.Diff(left, right);  
 // diffs: [{"op":"remove","path":"/c"}]
```
2. compare basic type and no duplicated elements json arrays index by index(orderly), this is default setting:
```C#
 string left =  "{\"a\":[1,2,3,4]}";
 string right = "{\"a\":[1,2,4,3]}"; 
 Differ differ = new Differ();
 JArray diffs = differ.Diff(left, right);  
 // diffs: [{"op":"replace","path":"/a/2","value":4},{"op":"replace","path":"/a/3","value":3}]
```
3. compare basic type and no duplicated elements json arrays not refer the order:
```C#
 bool NoOrderInBasicTypeValueJArray = true;
 string left =  "{\"a\":[1,2,3,4]}";
 string right = "{\"a\":[1,2,4,3]}"; 
 Differ differ = new Differ(NoOrderInBasicTypeValueJArray);
 JArray diffs = differ.Diff(left, right);  
 // diffs: []
```

* **ApplyAll**
```C#
string tStr = "{\"foo\":\"bar\",\"foo1\":{\"bar\":\"baz\",\"waldo\":\"fred\"},\"qux1\":{\"corge\":\"grault\"},\"foo2\":{\"bar\":\"baz\",\"waldo\":\"fred\"},\"qux2\":{\"corge\":\"grault\"},\"foo3\":\"bar\"}";
string pStr = "[{op: 'add', path: '/foonew', value: 'barnew'},{op: 'remove', path: '/foo'},{\"op\":\"move\",\"from\":\"/foo1/waldo\",\"path\":\"/qux1/thud\"},{\"op\":\"copy\",\"from\":\"/foo2/waldo\",\"path\":\"/qux2/thud\"},{op: 'replace', path: '/foo3', value: 'bar3'}]";

JToken target = JToken.Parse(tStr);
JArray patches = JArray.Parse(pStr);
JToken result = Apply.ApplyAll(target, patches);
// result: {"foo1":{"bar":"baz"},"qux1":{"corge":"grault","thud":"fred"},"foo2":{"bar":"baz","waldo":"fred"},"qux2":{"corge":"grault","thud":"fred"},"foo3":"bar3","foonew":"barnew"}
```

* **Add**
```C#
JToken target = JToken.Parse("{}");
JObject patch = JObject.Parse("{\"op\": \"add\", \"path\": \"/foo\", \"value\": \"bar\"}");
JToken result = Apply.Add(target, patch);
// result: {"foo":"bar"}
```

* **Remove**
```C#
JToken target = JToken.Parse("{\"foo\":\"bar\"}");
JObject patch = JObject.Parse("{\"op\": \"remove\", \"path\": \"/foo\", \"value\": \"bar\"}");
JToken result = Apply.Remove(target, patch);
// result: {}
```

* **Move**
```C#
JToken target = JToken.Parse("{\"foo\":{\"bar\":\"baz\",\"waldo\":\"fred\"},\"qux\":{\"corge\":\"grault\"}}");
JObject patch = JObject.Parse("{\"op\":\"move\",\"from\":\"/foo/waldo\",\"path\":\"/qux/thud\"}");
JToken result = Apply.Move(target, patch);
// result: {"foo":{"bar":"baz"},"qux":{"corge":"grault","thud":"fred"}}
```

* **Copy**
```C#
JToken target = JToken.Parse("{\"foo\":{\"bar\":\"baz\",\"waldo\":\"fred\"},\"qux\":{\"corge\":\"grault\"}}");
JObject patch = JObject.Parse("{\"op\":\"copy\",\"from\":\"/foo/waldo\",\"path\":\"/qux/thud\"}");
JToken result = Apply.Copy(target, patch);
// result: {"foo":{"bar":"baz","waldo":"fred"},"qux":{"corge":"grault","thud":"fred"}}
```

* **Replace**
```C#
JToken target = JToken.Parse("{\"foo\":\"barr\"}");
JObject patch = JObject.Parse("{\"op\": \"replace\", \"path\": \"/foo\", \"value\": \"bar\"}");
JToken result = Apply.Replace(target, patch);
// result: {"foo":"bar"}
```

* **Test**
```C#
JToken target = JToken.Parse("{\"baz\":\"qux\"}");
JObject patch = JObject.Parse("{\"op\":\"test\",\"path\":\"/baz\",\"value\":\"bar\"}");
JToken result = Apply.Test(target, patch); // throw an InvalidOperationException exception
```
