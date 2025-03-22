# Basic layout
The Model Definition has three levles, 

- Namespace
  - Type
    - Property

Each collection of properties are proceded by a type, and each collection of types are preceded by a namespace.

# Namespace
A namespace consits of the namespace definition line, and 0 or more types.

The namespace definition line begins with `#`, ends with `return`, and has the name in the middle

For example
```
#AccountsPayable

```

Each file must start with a namespace. If you want to specify a blank namespace, use just `#` followed by `return`

```
#

```

# Type
A type resides in a namespace and consits of the type definition line, and 0 or more properties.

The type definition line, at minimum, begines with `type`, ends with `return` and has the name in the middle.
```
#
type Customer

```

#### extends
The `extends` modifier allows specifying a Base Type. `extends` is followed by the name of the Base Type.

For Example
```
#
type SalesPerson extends Employee

```

By default, the Base Type will be interpreted as being in the same namespace as the Derived Type. To specify a namespaces, prefix the Base Type with the desired namespace followed by `.`

For Example
```
#Crm
type Salesperson extends Hr.Employee

```

If the Base Type is in the root namespace, prefix with just `.`
```
#Crm
type Salesperson extends .Employee

```

# Property
A property resides in a type and consits of the property definition line.

The property definition line, at minimum, contains `tab` `type` `name` `return`

Example
```
#
type Customer
  string Name

```

## Modifiers
***
The property definition has many modifiers. Below is a description of each

### Prefix Modifiers
Prefix modifiers change attributes about the property, such as Required, Nullable, Display Name, e.t.c.

They can be places in any order, with or without spaces, before the Type of the property is declared.

#### + (Primary Key)
The '+' indicates the property is the primary key for the type. If multiple properties are specified, then a composite primary key will be configured. With the property listed first having the higher precedence.

Example
```
#
type Customer
  +Guid Id

```

**Note:** When building an intersect type, you can add the `+` modifier to the navigation property definition.

Example
```
#
type Customer_SalesPerson
  +Customer Customer => SalesPersons
  +SalesPerson SalesPerson => Customers

```

#### * (Validation)
The `*` modifier indicates the property should be marked as required in the UI

Example
```
#
type Customer
  *string Name

```

#### [""] Display Name
The `[""]` modifier indicates the display name of the property in the UI.

Example
```
#
type OrderLineItem
  ["Price"] decimal UnitPrice

```

#### [m:] Max Length
The `[""]` modifier indicates the Max Length of a string.

Example
```
#
type Customer
  [m:30] string Description

```

#### [p:] Precision
The `[p:]` modifier is for use with floating point types, e.g. `float`, `double`, `decimal`. It specifies how many total digits will be stored. It is usually pared with `Scale`.

Example
```
#
type OrderLineItem
  [p:18][s:2]decimal UnitPrice

```

#### [s:] Scale
The `[s:]` modifier is for use with floating point types, e.g. `float`, `double`, `decimal`. It specifies how many digits will be after the decimal point. It is usually pared with `Precision`.

Example
```
#
type OrderLineItem
  [p:18][s:2]decimal UnitPrice

```

### Type Modifiers
The following modifiers target the `Type` portion of the property definition

#### [] Collection
Placing `[]` directly after the `Type`, with **no** whitespace, indicates the property type is a collction.

Example
```
#
type Customer
  Contact[] Contacts

```

#### ? Nullable
Placing `?` directly after the `Type`, or `[]` if it is a collection, with **no** whitespace, indicates the property type nullable.

Example
```
#
type Customer
  CustomerType? Type
  Contact[]? Contacts

```

### Navigation Modifiers
Navigation Modifiers come after `Name`

These modifiers configure the relationship between types. The relationship can be One-to-Many, Many-to-One, or One-to-One.

**Note:** Many-to-Many relationship need to be broken into One-to-Many/Many-to-One through the use of an intersect type.

#### `=> <Partner>` One-to-Many/Many-to-One
A few things about this modifier. 

First, whether it is a One-to-Many or Many-to-One depends on the `Type`.
 - If the `Type` is a collection (as denoted by `[]`), then it is a Many-to-One.
 - On the otherhand, if the `Type` is singular (lacking the `[]` annotation), then it is a One-to-Many.

Second, after the `=>` modifier comes the `<Partner>` section. This is the `Name` of the corresponding property on the specified `Type`.

Example - One-to-Many
```
#
type Customer
  Contact[] Contacts => Customer

```

Example - Many-to-One
```
#
type Contact
  Customer Customer => Contacts

```

In the preceeding examples, the `Type` and `Name` of the Many-to-One relationship matched, but that need not be the case.

A More Complex Example
```
#
type Order
  Employee CreatedBy => CreatedOrders

type Employee
  Order[] CreatedOrders => CreatedBy

```

It is also important to point out, that the navigation definition is only required on the Many-to-One side. If there will be no need to query from the One-to-Many side, then it need not be defined.

Abbreviated Complex Example
```
#
type Order
  Employee CreatedBy =>

type Employee

```

#### :`<PrincipalKey>|<ForeignKey>` Key specification
By default the Principal and Foreign Keys will be generated based on convention, but it is possible to override this behavior.

First the default convention, for the given specification
```
#
type Order
  Employee CreatedBy => CreatedOrders

type Employee
  Order[] CreatedOrders => CreatedBy

```

The `Foreign Key` would be `CreatedById` and the `Principal Key` would be `Id`. 

The `Foreign Key`'s orginiation is somewhat obvious, it is the Many-to-One side's `Name` with `Id` appended. The `Principal Key`'s is more obtuse, it is just `Id`, always and forever, because... thats the way I did it. (Really it has to do with the inheritance structure in the Repositories and Controllers)

You can of course override the defaults. This override will only be honored on the Many-to-One side. (The side with a Singular Type).

These are the overrides.
`Type:ForeignKey`<br>
`Partner:PrincipalKey`

You can can of course override one, or the other, or both, or neither.

Example of Key overrides
```
type Order
  Employee CreatedBy:EmployeeId => CreatedOrders:WorkerId

type Employee
  +Guid WorkerId
  Order[] CreatedOrders => CreatedBy

```


#### `<=> <Partner>` One-to-One
In a One-to-One relationship, the `Primary Key` of one `Type` is the `Foreign Key` and the `Primary Key` of the other `Type` is the `Principal Key`. I'll refer to the `Type` with the `Foreign Key` as the `Subordinate`. 

This modifier is very similar to the `=>`, but unlike that modifier, it will always be on a property `Type` that is `Singular`. 

Like the `=>` modifier, after the `<=>` modifier comes the `<Partner>` section. This is the `Name` of the corresponding property on the specified `Type`. The `<Parter>` section is only required on the `Subordinate`.

Unlike the `=>` modifier, the `Principal Key` and `Foreign Key` both default to `Id`. Just like the `=>` the `Keys` are only honor on the `Subordinate`

Example - One-to-One
```
#
type Employee
  User User <=> Employee

type User
  Employee Employee <=> User

```

In the preceeding examples, the `Type` and `Name` of the One-to-One relationship matched, but that need not be the case.

A More Complex Example
```
#
type Employee
  User Identity <=> Worker

type User
  Employee Worker <=> Identity

```

It is also important to point out, that the navigation definition is only required on side that contains the `Foriegn Key`. If there will be no need to query from the `Principal Key` side, then it need not be defined.

Abbreviated Complex Example
```
#
type Employee
  User Identity <=>

type User

```

You can also override the keys
```
#
type Employee
  +Guid EmployeeId
  User Identity:EmployeeId <=> Worker:UserId

type User
  +Guid UserId
  Employee Worker <=> Identity
```

### Other Modifiers

#### = Default Value
The default value modifer is placed at the end of the property line, before `return`. It overrides the default value of the property.

Example
```
#
type Customer
  int SortOrder = 10

```

**Note:** Below are the "Default" Default Values.

| Type | Value | 
| --- | --- |
| Nullable | No Default Value (The rest of the types in this table assume Non-Nullable) | 
| Non-Reference Primative | No Default Value |
| String | string.Empty |
| Object | null! |
| Collection | [] |

