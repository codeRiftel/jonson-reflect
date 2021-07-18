# jonson-reflect
jonson-reflect allows you to easily parse objects into JSON and vice-versa. It uses JSONType from [jonson](https://github.com/codeRiftel/jonson) as a JSON type representation.

## Alternative
If for some reason reflection isn't an option for you and you're still short on time to handle all this verbosity, then check out [meta-jonson](https://github.com/codeRiftel/meta-jonson) which will generate C# code for you.

## Considerations
* uses reflection
* considers List<T> as a json array
* considers Dictionary<string, T> as a json object
* note that it will serialize Dictionary<object, T>, but it won't read it back because key is not a string
* all types involved must have parameterless constructor (implicit counts), though exception is made for string
* field must be public to make it in JSONType
* property is not a field
* you can leave out null fields
* no protection from cycles at all

## Usage
Suppose you have this JSON in a **string** named **input**
```javascript
{
    "name": "foo",
    "age": 42,
    "dumb": true,
    "credentials": null,
    "repos": ["bar1", "bar2"]
}
```
Let's parse it.
```csharp
public struct Person {
    public string name;
    public int age;
    public bool dumb;
    public string credentials;
    public List<string> repos;
}

Person person = new Person();
Result<JSONType, JSONErr> personRes = Jonson.Parse(input, 1024);
if (personRes.IsErr()) {
    return;
}

person = Reflect.FromJSON(person, personRes.AsOk());
```
Let's generate JSON from instance.
```csharp
JSONType personType = Reflect.ToJSON(person);
string output = Jonson.Generate(personType);
```
