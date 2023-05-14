using LINQSuite;
using LINQSuite.Models;
using System.Text.Json;
// See https://aka.ms/new-console-template for more information

#region Basic Linq
Console.WriteLine(" I) Projection\n");
# region Projection : Select-SelectMany
Console.WriteLine("Projection-Select");
//With Select we create a projection from one item to another.
IEnumerable<string> stringsForSelect = new List<string> { "one", "two", "three", "four" };
// Will return { 3, 3, 5, 4 }
IEnumerable<int> resultForselect = stringsForSelect.Select(str => str.Length);
foreach (var to in resultForselect)
{
    Console.WriteLine(to);
}
Console.ReadKey();
Console.WriteLine("Projection-Select");
var objects = new List<SourceObject>
{
 new SourceObject(1),
 new SourceObject(2),
};
// [
// TargetObject { NumberAsString: "1" },
// TargetObject { NumberAsString: "2" },
// ]
var targetObjects = objects.Select(o => new TargetObjectClass()
{
    NumberAsString = o.Number.ToString()
}) ; 

foreach (var tob in targetObjects)
{
    Console.WriteLine(tob.NumberAsString);
}
Console.ReadKey();
Console.WriteLine("Projection-SelectMany");
// SelectMany is used to flatten lists
int[][] arrays = {
    new[] {1, 2, 3},
    new[] {4},
    new[] {5, 6, 7, 8,8},
    new[] {12, 14}
};
// Will return { 1, 2, 3, 4, 5, 6, 7, 8, 12, 14 }
IEnumerable<int> result = arrays.SelectMany(array => array);
foreach (var to in result)
{
    Console.WriteLine(to);
}
Console.ReadKey();
//You can also perform transformations on the constituent sequences, as shown in this example utilizing a list of lists:
Console.WriteLine("Projection-SelectMany-Distinct");
List<List<int>> lists = new List<List<int>> {
    new List<int> {1, 2, 3},
    new List<int> {12},
    new List<int> {5, 6, 5, 7},
    new List<int> {10, 10, 10, 12}
};
// Will return { 1, 2, 3, 12, 5, 6, 7, 10, 12 }
IEnumerable<int> resultForSelectMany = lists.SelectMany(list => list.Distinct()).ToList().Distinct();
foreach (var to in resultForSelectMany)
{
    Console.WriteLine(to);
}
Console.ReadKey();
Console.WriteLine("Projection-SelectMany");
var recipes = new List<Recipe>
{
 new Recipe("Pizza", new() { "Tomato Sauce, Basil" }),
 new Recipe("Hot Water", new() { "Water" }),
};
// [
// "Tomato Sauce", "Basil", "Water"
// ]
var allIngredients = recipes.SelectMany(r => r.Ingredients);
foreach (var to in allIngredients)
{
    Console.WriteLine(to);
}
#endregion
Console.ReadLine();
Console.Clear();
Console.WriteLine(" II) Materialisation/Conversion\n");
# region Materialisation/Conversion: ToLookUp-ToDictionary-ToList-ToArray-Cast-ToHashSet

//ToLookUp
//This methods creates a lookup. A lookup is defined that we 
//have a key which can point to list of objects (1 to n relation). 
//The first argument takes the "key"-selector. The second 
//selector is the "value". This can be the object itself or a 
//property of the object itself. At the end we have a list of 
//distinct keys where the values share that exact key. A 
//LookUp-object is immutable. You can't add elements 
//afterwards.
Console.WriteLine("Tolookup");
var products = new[]
{
 new Product("Smartphone", "Electronic"),
 new Product("PC", "Electronic"),
 new Product("Apple", "Fruit")
};

// IGrouping<string, Product>
// [
// "Electronic": [ "Smartphone", "PC"],
// "Apple": [ "Fruit"]
// ]
var lookup = products.ToLookup(k => k.Category, elem => elem);
// Iterate through each IGrouping in the Lookup and output the contents.
foreach (IGrouping<string, Product> productGroup in lookup)
{
    // Print the key value of the IGrouping.
    Console.WriteLine("Product Group Key    {0}", productGroup.Key);
    // Iterate through each value in the IGrouping and print its value.
    foreach (Product str in productGroup)
        Console.WriteLine("Product Group Name    {0}", str.Name);
}

//ToDictionary works similar to ToLookup with a key 
//difference. The ToDictionary method only allows 1 to 1 
//relations. If two items share the same key, it will result in an 
//exception that the key is already present. Also the 
//dictionary can be mutated afterwards (for example with the 
//Add method).

//ToList/ToArray
//As mentioned at the beginning, objects of the type 
//Enumerable are not evaluated directly, but only when they 
//are materialised. Beside quantifiers like Count or Sum there 
//is also the possibility to pack the complete enumeration 
//into a typed collection / array (ToArray) or list(ToList). 
//With this we create the enumeration in memory at exactly
//this time

//Cast
Console.WriteLine("M/C-Cast ");
System.Collections.ArrayList fruitsCast = new System.Collections.ArrayList();
fruitsCast.Add("mango");
fruitsCast.Add("apple");
fruitsCast.Add("lemon");

IEnumerable<string> queryCast =
    fruitsCast.Cast<string>().OrderBy(fruit => fruit).Select(fruit => fruit);

foreach (string fruit in queryCast)
{
    Console.WriteLine(fruit);
}

//Hashset
Console.WriteLine("M/C-Hashset ");
var listHS = Enumerable.Range(1, 100).Select(i=> new
{
    i,
    j = i + 1
});
var resultSet = listHS.ToHashSet();

foreach (var a in resultSet)
{
    Console.WriteLine(a.i +","+ a.j);
}
#endregion
Console.ReadLine();
Console.Clear();
Console.WriteLine(" III) Merging\n");
# region Merging: Join-Group Join
// looks into operations which are responsible of 
//merging two or more enumerations into one object.

Console.WriteLine("Merging:Join");
//Join  SQL inner Join.
//Join works similar to a SQL Left-Join. We have two sets 
//we want to join. The next two arguments are the "key" 
//selectors of each list. What Join basically does is it takes 
//every element in list A and compares it with the given "keyselector" against the key-selector of list b. If it matches we 
//can create a new object C, which can consist out of those 
//two elements.
var fruitsForJoin = new[]
{
 new FruitForJoin(1, "Banana", 89),
 new FruitForJoin(2, "Apple", 51),
 new FruitForJoin(3, "Peach", 55),
};
var classification = new[]
{
 new FruitClassification(1, "Magnesium-rich"),
 new FruitClassification(2, "C-rich"),
 new FruitClassification(4, "B-rich")
};
// { Name = Banana, Classification = Magnesium-rich }
var fruitWithClassification = fruitsForJoin.Join(classification,
 f => f.FruitId, 
 c => c.FruitId,
 (f, c) => new { f.Name, Classification = c.Classification });
foreach (var t in fruitWithClassification) Console.Write(t);
Console.WriteLine();
Console.WriteLine("Merging:Left Join");
var query =
    from fruit in fruitsForJoin
    join classif in classification on fruit.FruitId equals classif.FruitId into gj
    from subclasification in gj.DefaultIfEmpty()
    select new
    {
        fruit.Name,
        Classification = subclasification?.Classification ?? string.Empty
    };

foreach (var v in query)
{
    Console.WriteLine($"{v.Name + ":",-15}{v.Classification}");
}


Console.WriteLine("Merging:right Join");
var queryrj =
    from classif in classification 
    join fruit in fruitsForJoin
    on classif.FruitId equals fruit.FruitId into grj
    from subfruit in grj.DefaultIfEmpty()
    select new
    {
        Name= subfruit?.Name ?? string.Empty,
        Classification = classif.Classification
    };

foreach (var v in queryrj)
{
    Console.WriteLine($"{v.Name + ":",-15}{v.Classification}");
}

Console.WriteLine("Merging:full Outer Join");
var fullOuterJoin = query.Union(queryrj);

foreach (var v in fullOuterJoin)
{
    Console.WriteLine($"{v.Name + ":",-15}{v.Classification}");
}

Console.WriteLine("Merging:Cartesian Product");
var resultcp =from p1 in fruitsForJoin
            from p2 in classification
            select new { Name=p1?.Name ?? string.Empty, Classification=p2?.Classification ?? string.Empty };
foreach (var v in resultcp)
{
    Console.WriteLine($"{v.Name + ":",-15}{v.Classification}");
}
// Group Join 
Console.WriteLine("Merging:Group Join");
var studentList = new[] {
    new Student( 1,"John", 1 ),
    new Student( 2,"Moin", 1 ),
    new Student( 3,"Bill", 2 ),
    new Student( 4,"Ram", 2),
    new Student( 5,"Ron",1)
};

var standardList = new[] {
    new Standard( 1, "Standard 1"),
    new Standard( 2, "Standard 2"),
    new Standard( 3,"Standard 3")
};

var innerJoin = studentList.Join(// outer sequence 
                      standardList,  // inner sequence 
                      student => student.StandardID,    // outerKeySelector
                      standard => standard.StandardID,  // innerKeySelector
                      (student, standard) => new  // result selector
                      {
                          StudentName = student.StudentName,
                          StandardName = standard.StandardName
                      });

foreach (var obj in innerJoin) Console.WriteLine("{0} - {1}", obj.StudentName, obj.StandardName);

var groupJoin = standardList.GroupJoin(studentList,  //inner sequence
                                std => std.StandardID, //outerKeySelector 
                                s => s.StandardID,     //innerKeySelector
                                (std, studentsGroup) => new // resultSelector 
                                {
                                    Students = studentsGroup,
                                    StandarFulldName = std.StandardName
                                });
foreach (var item in groupJoin)
{
    Console.WriteLine(item.StandarFulldName);

    foreach (var stud in item.Students)
        Console.WriteLine(stud.StudentName);
}



//Zip
//With Zip we "merge" two lists by a given merge function. 
//We merge objects together until we run out of objects on 
//either of the lanes. As seen in the example: The first lane 
//has 2 elements, the second has 3. Therefore the result set 
//contains only 2 elements
var letters = new[] { "A", "B", "C", "D", "E" };
var numbersForZip = new[] { 1, 2, 3 };
// [ "A1", "B2", "C3" ]
var merged = letters.Zip(numbersForZip, (l, n) => l + n);
foreach (var t in merged) Console.Write("'" + t + "'");
Console.WriteLine();

#endregion
Console.ReadLine();
Console.Clear();
Console.WriteLine(" IV) Filtering\n");
# region Filtering: Where-Distinct-DistinctBy-OfType
//WHERE
//With Where we can filter a given list based on our 
//condition. The method accepts a Predicate. That means we 
//define a filter function which then gets applied object by 
//object. If the filter evaluates to true, the element will be 
//returned in the new enumeration
var listW = new List<int>();
listW.Add(1);
listW.Add(2);
// Get even numbers
// Result: [ 2 ]
var evenNumbers = listW.Where(n => n % 2 == 0);

Console.WriteLine("Filtering-Distinct ");
//Distinct returns a new enumerable where all duplicates are 
//removed, kind of like a Set. Be careful that for reference 
//type the default is to check for equality of references,
//which can lead to false results. The result set can be the 
//same or smaller
Product[] productsW = {
    new Product("apple","9" ),
    new Product("orange","4" ),
    new Product("apple","9"),
    new Product("lemon","12")
 };
// Exclude duplicates.

IEnumerable<Product> noduplicates =
    productsW.Distinct(new LINQSuite.ProductComparer());

foreach (var product in noduplicates)
    Console.WriteLine(product.Name + " " + product.Category);

Console.WriteLine("Filtering-DistinctBY ");
//DistinctBy works similar to Distinct but instead of the
//level of the object itself we can define a projection to a 
//property where we want to have a distinct result set
var peopleDB = new List<Person>
{
 new Person("Steven", 31),
 new Person("Katarina", 29),
 new Person("Nils", 31)
};
// [
// Person { Name = Steven, Age = 31 },
// Person { Name = Katarina, Age = 29 }
// ]
var uniqueAgedPeople = peopleDB.DistinctBy(p => p.Age);
foreach (var person in uniqueAgedPeople)
    Console.WriteLine(person.Name + " " + person.Age);

//OfType
//OfType checks every element in the enumeration if it is of 
//a given type (also inherited types count as that given type) 
//and returns them in a new enumeration.That helps
//especially if we have untyped arrays(object) or we want a 
//special subclass of the given enumeration
Console.WriteLine("Filtering-OfType ");
var fruitsOT = new List<FruitF>
{
 new Banana(),
 new Apple()
};
// [
// Apple { }
// ]
var apples = fruitsOT.OfType<Apple>();
foreach (var fruit in apples)
    Console.WriteLine(fruit);
#endregion
Console.ReadLine();
Console.Clear();
Console.WriteLine(" V) Aggregation\n");
# region Aggregation: Aggregate-Max-MaxBy-Min-MinBy

//Aggregation describes the process of reducing the whole 
//enumeration to a single value.


//.Aggregate
//Aggregate, also known as reduce, aggregates/reduces 
//all elements into a scalar value. A prime example is the 
//sum of a list. We start with 0 and add each element on top 
//until we enumerated through our enumeration. Aggregates 
//first parameter is the start value. An empty enumeration 
//will result in returning your start value
var numbers = new[] { 1, 2, 3 };
// 6
var sum = numbers.Aggregate(0, (curr, next) => curr + next);
// 6
var sumLinq = numbers.Sum();
Console.WriteLine(sumLinq);

//Max
var max = new[] { 1, 2, 3 }.Max();
//3
Console.WriteLine(max);

//MaxBy

var people = new[]
{
 new Person("Steven", 31),
 new Person("Jean", 22)
};
// Person { Name: Steven, Age: 31 }
var oldest = people.MaxBy(p => p.Age);

Console.WriteLine(oldest);

#endregion
Console.ReadLine();
Console.Clear();
Console.WriteLine(" VI) Quantification\n");
# region Quantification: Any-All-SequenceEqual
//Those operations want to measure the quantity of something

//Any
//Any checks if at least one element satisfies your condition. 
//If so, it returns true. If there is no element that meets the 
//condition, then it returns false. Any also immediately stops 
//processing once it founds one element. It returns false if 
//the given enumeration is empty
var fruits = new[]
{
 new Fruit("Banana", 89),
 new Fruit("Apple", 51),
};
// true
var hasDenseFood = fruits.Any(f => f.CaloriesPer100Gramm > 80);
Console.WriteLine(hasDenseFood);

//All
//As the name implies checks if All of your elements in the 
//list satisfy a certain condition.

// false
var hasDenseFoodAll = fruits.All(f => f.CaloriesPer100Gramm > 80);
Console.WriteLine(hasDenseFoodAll);

//SequenceEquals
//SequenceEquals checks if two sequences are equal. Equal 
//means they have the same amount of entries inside the 
//enumeration as well as all elements are equal. It uses the 
//default equality comparer. Two empty lists are also equal.
//There is an optional second parameter which allows to pass 
//in an IEqualityComparer. That is useful if you don’t have 
//control over the type and therefore can’t override Equals. 
//By default reference types are compared by their 
//references against each other, which is not always what you 
//want.



var numbersSE = new[] { 1, 2, 3, 4 };
var moreNumbersSE = new[] { 1, 2 , 4 , 3 };
// false
var equal = numbersSE.SequenceEqual(moreNumbersSE);
Console.WriteLine(equal);
#endregion
Console.ReadLine();
Console.Clear();
Console.WriteLine(" VII) Element\n");
# region Element : First-FirstOrDefault-Single-SingleOrDefault
//looks closer how to retrieve a specific item 
//from the enumeration
//First 

//First returns the first occurrence of an enumeration. Even 
//if there are elements later it always returns immediately 
//after the first found item. If no element is found, it throws 
//an exception.

//FirstOrDefault /SingleOrDefault

//If no element is found in the given enumeration it returns it 
//the default (for reference types null and for value types the 
//given default like 0 for an integer). Since.NET6 we can pass 
//in what "default" means to us. Therefore we can have nonnullable
//reference types if we wish or any given number /float / string

var peopleF = new[]
{
 new Person("Steven", 31),
 new Person("Melissa", 32),
 new Person("Dan", 28)
};

// Person { Name: Steven, Age: 31 }
var firstOver30 = peopleF.First(p => p.Age > 30);
Console.WriteLine(firstOver30);
// null, as the default of a reference type is null
var steven = peopleF.FirstOrDefault(p => p.Name == "Dan");
Console.WriteLine(steven);
#endregion
Console.ReadLine();
Console.Clear();
Console.WriteLine(" VIII) Grouping\n");
# region Grouping :GroupBy
//ToLookUp
//GroupBy groups the enumeration by a given projection / 
//key. All elements which share this exact key get grouped 
//together. It is almost identical to ToLookup with a very big 
//difference. GroupBy means "I am building an object to 
//represent the question 'what would these things look like if 
//I organised them by group?'" Calling ToLookup means "I 
//want a cache of the entire thing right now organised by 
//group.”

// GroupBy creates an IEnumerable<IGrouping<string, Product>>
// This is a big difference to ToLookup where we don't have
// the "wrapping" IEnumerable
// [
// "Electronic": [ "Smartphone", "PC"],
// "Apple": [ "Fruit"]
// ]
var lookupGB = products.GroupBy(k => k.Category, elem => elem);
// Iterate through each IGrouping in the Lookup and output the contents.
foreach (IGrouping<string, Product> productGroup in lookupGB)
{
    // Print the key value of the IGrouping.
    Console.WriteLine(productGroup.Key);
    // Iterate through each value in the IGrouping and print its value.
    foreach (Product str in productGroup)
        Console.WriteLine("    {0}", str.Name);
}

#endregion
Console.ReadLine();
Console.Clear();
Console.WriteLine(" VIV) Set\n");
# region Set :Union-Intersect
//Union
Console.WriteLine("SET");
Console.WriteLine("Union:");
var numbers1 = new[] { 1, 1, 2 };
var numbers2 = new[] { 2, 3, 4 };
// [ 1, 2, 3, 4 ]
var resultUnion = numbers1.Union(numbers2);
foreach(var r in resultUnion)Console.WriteLine(r);
Console.WriteLine("Intersect:");
//Intersect
var resultIntersect = numbers1.Intersect(numbers2);
foreach (var r in resultIntersect) Console.WriteLine(r);
Console.WriteLine("Except:");
IList<string> strList1 = new List<string>() { "One", "Two", "Three", "Four", "Five" };
IList<string> strList2 = new List<string>() { "Four", "Five", "Six", "Seven", "Eight" };
var resultExcept = strList1.Except(strList2);

foreach (string str in resultExcept)
    Console.WriteLine(str);

#endregion
Console.ReadLine();
Console.Clear();
Console.WriteLine(" VV) Sequence\n");
# region Sequence :Chunk-Repeat-Reverse-Range
//Chunk
var list = Enumerable.Range(1, 100);
var chunkSize = 10;
foreach (var chunk in list.Chunk(chunkSize)) //Returns a chunk with the correct size. 
{
    foreach (var item in chunk)
    {
        Console.WriteLine(item);
    }
}
foreach (var chunk in list.Chunk(chunkSize)) //Returns a chunk with the correct size. 
{
    Parallel.ForEach(chunk, (item) =>
    {
        //Do something Parallel here. 
        Console.WriteLine(item);
    });
}
//Repeat
IEnumerable<string> strings =
    Enumerable.Repeat("I like programming.", 5);

foreach (String str in strings)
{
    Console.WriteLine(str);
}

//reverse
char[] apple = { 'a', 'p', 'p', 'l', 'e' };

char[] reversed = apple.Reverse().ToArray();

foreach (char chr in reversed)
{
    Console.Write(chr + " ");
}
Console.WriteLine();

/*
 This code produces the following output:

 e l p p a
*/

//Range
// Generate a sequence of integers from 1 to 10
// and then select their squares.
IEnumerable<int> squares = Enumerable.Range(1, 10).Select(x => x * x);

foreach (int num in squares)
{
    Console.WriteLine(num);
}

/*
 This code produces the following output:

 1
 4
 9
 16
 25
 36
 49
 64
 81
 100
*/

//Prepend
// Creating a list of numbers
List<int> numbersP = new List<int> { 1, 2, 3, 4 };

// Trying to prepend any value of the same type
numbersP.Prepend(0);

// It doesn't work because the original list has not been changed
Console.WriteLine(string.Join(", ", numbersP));

// It works now because we are using a changed copy of the original list
Console.WriteLine(string.Join(", ", numbersP.Prepend(0)));

// If you prefer, you can create a new list explicitly
List<int> newNumbers = numbersP.Prepend(0).ToList();

// And then write to the console output
Console.WriteLine(string.Join(", ", newNumbers));

// This code produces the following output:
//
// 1, 2, 3, 4
// 0, 1, 2, 3, 4
// 0, 1, 2, 3, 4

//Empty
string[] names1 = { "Hartono, Tommy" };
string[] names2 = { "Adams, Terry", "Andersen, Henriette Thaulow",
                      "Hedlund, Magnus", "Ito, Shu" };
string[] names3 = { "Solanki, Ajay", "Hoeing, Helge",
                      "Andersen, Henriette Thaulow",
                      "Potra, Cristina", "Iallo, Lucio" };

List<string[]> namesList =
    new List<string[]> { names1, names2, names3 };

// Only include arrays that have four or more elements
IEnumerable<string> allNames =
    namesList.Aggregate(Enumerable.Empty<string>(),
    (current, next) => next.Length > 3 ? current.Union(next) : current);

foreach (string name in allNames)
{
    Console.WriteLine(name);
}

/*
 This code produces the following output:

 Adams, Terry
 Andersen, Henriette Thaulow
 Hedlund, Magnus
 Ito, Shu
 Solanki, Ajay
 Hoeing, Helge
 Potra, Cristina
 Iallo, Lucio
*/
#endregion
Console.ReadLine();
Console.Clear();
Console.WriteLine(" VVI) Order\n");
# region Order: OrderBy-ThenBy-ThenByDescending
Console.WriteLine("ORDER");
Console.WriteLine("ThenBy:");
var thenByResult = studentList.OrderBy(s => s.StudentName).ThenBy(s => s.StandardID);
foreach (var std in thenByResult) Console.WriteLine("Name: {0}, StandardID:{1}", std.StudentName, std.StandardID);

Console.WriteLine("ThenByDescending:");
var thenByDescResult = studentList.OrderBy(s => s.StudentName).ThenByDescending(s => s.StandardID);
foreach (var std in thenByDescResult)
    Console.WriteLine("Name: {0}, StandardID:{1}", std.StudentName, std.StandardID);
#endregion

#endregion

#region Advanced Linq

#region GroupBy
Console.WriteLine("");
Console.WriteLine("Advanced Linq");
//GroupBy
Console.WriteLine("");
Console.WriteLine("GroupBy");
var employeeDepartmentGroups = TestData.Employees.GroupBy(x => x.Department);
foreach (IGrouping<string, Employee> employeeGroup in employeeDepartmentGroups)
{
    // Print the key value of the IGrouping.
    Console.WriteLine(employeeGroup.Key);
    // Iterate through each value in the IGrouping and print its value.
    foreach (Employee str in employeeGroup)
        Console.WriteLine("    {0}", str.Name);
}


//GroupBy-Where
Console.WriteLine("");
Console.WriteLine("GroupBy-Where");
var employeeDepartmentGroups2 = TestData.Employees.Where(z=>z.Salary<100000).GroupBy(x => x.Department, elem => elem);
foreach (IGrouping<string, Employee> employeeGroup in employeeDepartmentGroups2)
{
    // Print the key value of the IGrouping.
    Console.WriteLine(employeeGroup.Key);
    // Iterate through each value in the IGrouping and print its value.
    foreach (Employee str in employeeGroup)
        Console.WriteLine("    {0}    {1}", str.Name,str.Salary);
}

//GroupBy - Where - Select
Console.WriteLine("");
Console.WriteLine("GroupBy-Where-Select");
var employeeDepartmentGroups2WithSelect = TestData.Employees.Where(z => z.Salary < 100000).GroupBy(x => x.Department, elem => elem)
    .Select(grouped => new {
        Catg = grouped.Key,
        Count = grouped.Count()
    }); 
foreach (var employeeGroup in employeeDepartmentGroups2WithSelect)
{
    // Print the key value  and  the count
    Console.WriteLine(employeeGroup.Catg +"  ("+ employeeGroup.Count + ")");
    
}

// GroupBy - Where - Select-Named type
Console.WriteLine("");
Console.WriteLine("GroupBy-Where-Select-Named type");
var employeeDepartmentGroups2WithSelectNT = TestData.Employees.Where(z => z.Salary < 100000).GroupBy(x => x.Department, elem => elem)
    .Select(grouped => new  KeyCount(){
        Key = grouped.Key,
        Count = grouped.Count()
    });
foreach (var employeeGroup in employeeDepartmentGroups2WithSelectNT)
{
    // Print the key value  and  the count
    Console.WriteLine(employeeGroup.Key + "  (" + employeeGroup.Count + ")");
}

// GroupBy With Composite Key
Console.WriteLine("");
Console.WriteLine("GroupBy With Composite Key");
var employeeDepartmentGroupsWithCompositeKey = TestData.Employees.GroupBy(x => new { x.Department, x.Salary });

foreach (var employeeGroup in employeeDepartmentGroupsWithCompositeKey)
{
    // Print the key value of the IGrouping.
    Console.WriteLine(employeeGroup.Key);
    // Iterate through each value in the IGrouping and print its value.
    foreach (Employee str in employeeGroup)
        Console.WriteLine("    {0}", str.Name);
}
//GroupBy With Salary level Key
Console.WriteLine("");
Console.WriteLine("GroupBy With Salary level Key");

var employeeDepartmentGroupsKeySalaryLevel = TestData.Employees.GroupBy(employee =>
{
    var salaryLevel = employee.Salary < 50000 ? "Entry-Level" :
                      employee.Salary >= 50000 && employee.Salary <= 85000 ? "Mid-Level" :
                      "Senior-Level";

    return salaryLevel;
});
foreach (IGrouping<string, Employee> employeeGroup in employeeDepartmentGroupsKeySalaryLevel)
{
    // Print the key value of the IGrouping.
    Console.WriteLine("Salary - "+employeeGroup.Key);
    // Iterate through each value in the IGrouping and print its value.
    foreach (Employee str in employeeGroup)
        Console.WriteLine("    {0}", str.Name);
}
//GroupBy Aggregate Report
Console.WriteLine("");
Console.WriteLine("GroupBy Aggregate Report");

var employeeDepartmentAggregateReport = TestData.Employees.GroupBy(x => x.Department).Select(group => new
{
    DepartmentName = group.First().Department,
    TotalDepartmentSalaryCosts = group.Sum(empl => empl.Salary),
    AverageSalaryCosts = group.Average(empl => empl.Salary)
});

foreach (var employeeGroup in employeeDepartmentAggregateReport)
{
    // Print the key value  and  Total salary
    Console.WriteLine(employeeGroup.DepartmentName + " Total salary  (" + employeeGroup.TotalDepartmentSalaryCosts + ")" + " average salary  (" + employeeGroup.AverageSalaryCosts + ")");

}
#endregion
#region OfType
Console.WriteLine("");
Console.WriteLine("OfType");
var admins = TestData.MixedEmployees.OfType<Administrator>();
var k= admins.ToList().ConvertAll(d => JsonSerializer.Serialize(d));
foreach (var str in admins)
{
    Console.WriteLine(str.Name);
}
Console.WriteLine("");
foreach (var str in k)
{
    Console.WriteLine(str);
}
#endregion
#region Join
Console.WriteLine("");
Console.WriteLine("Join");
var join =  TestData.Employees.Join(
            TestData.Directors,
            em => em.Department,
            dir => dir.DepartmentResponsibleFor,
            (em, dir) => new { EmployeeName = em.Name, DirectorName = dir.Name, Department = em.Department }).OrderBy(x=>x.Department);

foreach (var employeeGroup in join)
{
    // Print the key value  and  the count
    Console.WriteLine("Department : "+ employeeGroup.Department+ " Employee Name : " + employeeGroup.EmployeeName + " DirectorName : " + employeeGroup.DirectorName  );
}
#endregion
#region Group Join Inner
Console.WriteLine("");
Console.WriteLine("group Join(inner)");
Console.WriteLine("");
var groupJoinInner = TestData.Directors.GroupJoin(TestData.Employees,
    dir => dir.DepartmentResponsibleFor,
    em => em.Department,
    (dir, emGroup) => new
    {
        dir.DepartmentResponsibleFor,
        dir.Name,
        EmployeeGroup = emGroup
    });

foreach (var employeeGroup in groupJoinInner)
{
    // Print the key value  and  the count
    Console.WriteLine("Department : " + employeeGroup.DepartmentResponsibleFor + " Director - " + employeeGroup.Name);
    foreach (var emGroup in employeeGroup.EmployeeGroup)
    {
        Console.WriteLine("Employee : " + emGroup.Name);
    }
    Console.WriteLine("");
}
#endregion
#region Group Join (Left outer)
Console.WriteLine("");
Console.WriteLine("group Join(left outer)");
Console.WriteLine("");
var groupJoinLeft = TestData.Directors.GroupJoin(TestData.Employees,
    dir => dir.DepartmentResponsibleFor,
    em => em.Department,
    (dir, emGroup) => new
    {
        dir.DepartmentResponsibleFor,
        dir.Name,
        EmployeeGroup = emGroup.DefaultIfEmpty(new() { Name = "No Name" })
    });

foreach (var employeeGroup in groupJoinLeft)
{
    // Print the key value  and  the count
    Console.WriteLine("Department : " + employeeGroup.DepartmentResponsibleFor + " Director - " + employeeGroup.Name);
    foreach (var emGroup in employeeGroup.EmployeeGroup)
    {
        Console.WriteLine("Employee : " + emGroup.Name);
    }
    Console.WriteLine("");
}
#endregion
#region AsQuerable
Console.WriteLine("");
Console.WriteLine("AsQuerable");
var employeeQueryable = TestData.Employees.AsQueryable();

foreach (var str in employeeQueryable)
{
    Console.WriteLine(str.Name);
}
#endregion
#region sort
Console.WriteLine("");
Console.WriteLine("Sort");
var listForSort = TestData.Employees;
listForSort.Sort((p1, p2) => string.Compare(p1.Name, p2.Name, true));
foreach (var str in listForSort)
{
    Console.WriteLine(str.Name);
}
#endregion
#region OrderBy
Console.WriteLine("");
Console.WriteLine("OrderBy with Comparer");

var listForOrderBy = TestData.Employees.OrderBy(n => n.Name, new NameComparer());
foreach (var str in listForOrderBy)
{
    Console.WriteLine(str.Name);
}
#endregion

#endregion

#region Types
record SourceObject(int Number);
record TargetObject(string NumberAsString);
class TargetObjectClass {
   public string NumberAsString { get; set; }
}

record Recipe(string Name, List<string> Ingredients);
record Fruit(string Name, int CaloriesPer100Gramm);
record Person(string Name, int Age);
record FruitForJoin(int FruitId, string Name, int CaloriesPer100Gramm);
record FruitClassification(int FruitId, string Classification);
record Student(int StudentID, string StudentName, int StandardID);
record Standard(int StandardID, string StandardName);
record Product(string Name, string Category);
record FruitF;
record Banana : FruitF;
record Apple : FruitF;

#endregion



