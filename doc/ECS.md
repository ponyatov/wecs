# ECS
## Entity-Component-System

шаблон проектирования, который позволяет эффективно управлять объектами в игре, разделяя
- данные (компоненты), 
- сущности (объекты) 
- и логику (системы).

**С ECS ты просто садишься и делаешь приложение**, а не воюешь с архитектурой проекта. Нет нужды строить большие и “красивые” иерархии, продумывать кучу связей и париться про “X же не должен знать про Y”. При этом принципы ECS защищают тебя(не на 100%, ессесно) от безвыходной ситуации, в которую заводит плохая архитектура, когда дальнейшее развитие проекта становится очень болезненным. И даже если всё таки что-то пошло не так - рефакторинг в ECS совсем не проблема.

- **Компоненты** (например, `Health`, `Flyable`, `Boss`) добавляются к сущностям **динамически**.
- Нет жесткой иерархии — любая сущность может получить любые компоненты.
- Логика изолирована в **системах**, которые работают только с нужными компонентами.
- Новые механики = новые системы, **не затрагивающие старый код**.
- Данные хранятся **плотными массивами** (каждый компонент — отдельный массив).
	- Системы обрабатывают только нужные компоненты, что **уменьшает промахи кэша** и ускоряет выполнение.
	- Системы **автоматически параллелятся**, если они не конфликтуют по данным.
- Компоненты хранятся **только там, где нужны**:
	- **Нет "лишних" полей** — экономия памяти.
- **ООП лучше подходит**
	- Когда **данные и логика тесно связаны** (например, UI).

[[Compose Dungeon Denizens]]

## Rust
- [[bevy]]
- https://github.com/amethyst/specs
	- https://specs.amethyst.rs/docs/tutorials/

##

- MICHAEL-F-BRYAN<br>[A Thought Experiment:<br>Using the ECS Pattern Outside of Game Engines](https://adventures.michaelfbryan.com/posts/ecs-outside-of-games/)
- https://leopotam.com/2/
- Eforen Creates **UECS0** great intro
	- [What is an Entity Component System and Why should I care about ECS](https://www.youtube.com/watch?v=OJmVBo5HGOY)
- [ECS in UI](https://www.youtube.com/watch?v=nu8JJEJtsVE) (ru)
- [Шаблон проектирования Entity-Component-System — реализация и пример игры](https://habr.com/ru/post/343778/)

## What Is An Entity-Component-System?
https://adventures.michaelfbryan.com/posts/ecs-outside-of-games/#what-is-an-entity-component-system

- follows the _composition over inheritance_ principle
- *every object* in a game’s scene is an **entity** labeled with id
- entity consists of one or more **components** which *add behavior or functionality*
- behavior of an entity can be changed at runtime by adding or removing components
- **systems** runs as threads performing global actions on the every entity group marked with component associated with the corresponding system

or

- **Entity**: The entity is a general purpose object. Usually, it only consists of a **unique id** tags every coarse gameobject as a separate item.
- **Component**: The raw data *for one aspect* of the object, and how that labeled object interacts with the world.
- **System**: Each System runs continuously (like having its own private thread) and performs global actions on every Entity that possesses a Component of the same aspect as that System.

### Implementations

[[Rust/specs]]

## Inheritance isn’t Always the Best Tool for the Job
https://adventures.michaelfbryan.com/posts/ecs-outside-of-games/#inheritance-isnt-always-the-best-tool-for-the-job

**cross-cutting concerns** in CADs:
- Graphical entities (e.g. `Point`, `Line`, `Spline`) which are rendered to the screen
- Non-graphical entities which impart semantics to the drawing (individually managed `Layer`s)
- Both graphical and non-graphical entities can be `frozen` (made immutable) or `hidden` (made invisible)
- Entities can be given a `name` so users associate them with a concept
- graphical entities need different information for how to be rendered (e.g. a `Line` might just have a `stroke_colour`, while a `Circle` may also have a `fill_colour`)



## Creating an ECS-based CAD Library
## Rendering
## Bounding Boxes
## Conclusion


## [[specs]]

- **entity** only identifies some object
- **component**  just a chunk of datat holds *partial* state for a single **entity**
	- **components** are _associated_ with entities;
	- you can just insert components into **entity** , whenever you like.
- **system** contains active/executable logic or behavior that 
	- work by iterating over some **component** for all related entities

```Java
public class Player extends Character {
    private final Transform transform;
    private final Inventory inventory;
}
```
```Java
public class Npc extends Character {
    private final Transform transform;
    private final Inventory inventory; // component
    private final boolean isFriendly;  // component
}
```

[[ECS/entity]]:

```py
tom, bob = Player('tom'), Player('bob')
players  = [tom, bob]
```

[[ECS/component]]:

```Java
Transform transform;
Inventory inventory;
boolean   isFriendly;
```

[[ECS/system]]:

```Python
def transformer_system():
	for i in players:
		i.transform += (dx,dy)
```


## from scratch

https://ianjk.com/ecs-in-rust/

[[SDR]]

### Creating the World

#### [[ECS/component|components]]

```Rust
/// entity name
#[derive(Debug)]
struct Name(&'static str);
```
```Rust
/// character health
#[derive(Debug)]
struct Health(u8);
```
```Rust
/// positon (in virtual coordinates)
#[derive(Debug)]
struct Pos(i16, i16);
```

#### global World storage

holds all components indexed by **entity id** (or [[None]])

```Rust
/// global world storage
#[derive(Debug)]
struct World {
    name: Vec<Option<Name>>,
    health: Vec<Option<Health>>,
    pos: Vec<Option<Pos>>,
}
```
```Rust
impl World {
    pub fn new() -> Self {
        World {
            name: Vec::new(),
            health: Vec::new(),
            pos: Vec::new(),
        }
    }
}
```
