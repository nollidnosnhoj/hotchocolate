using System.Collections.Generic;
using System.Threading.Tasks;
using CookieCrumble;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;

namespace HotChocolate.Data.Projections;

public class QueryableProjectionFilterTests
{
    private static readonly Bar[] _barEntities =
    {
        new()
        {
            Foo = new Foo
            {
                BarShort = 12,
                BarBool = true,
                BarEnum = BarEnum.BAR,
                BarString = "testatest",
                NestedObject = new BarDeep { Foo = new FooDeep { BarShort = 12, BarString = "a" } },
                ObjectArray = new List<BarDeep>
                {
                    new() { Foo = new FooDeep { BarShort = 12, BarString = "a" } }
                }
            }
        },
        new()
        {
            Foo = new Foo
            {
                BarShort = 14,
                BarBool = true,
                BarEnum = BarEnum.BAZ,
                BarString = "testbtest",
                NestedObject = new BarDeep { Foo = new FooDeep { BarShort = 12, BarString = "d" } },
                ObjectArray = new List<BarDeep>
                {
                    new() { Foo = new FooDeep { BarShort = 14, BarString = "d" } }
                }
            }
        }
    };

    private static readonly BarNullable[] _barNullableEntities =
    {
        new()
        {
            Foo = new FooNullable
            {
                BarShort = 12,
                BarBool = true,
                BarEnum = BarEnum.BAR,
                BarString = "testatest",
                ObjectArray = new List<BarNullableDeep?>
                {
                    new() { Foo = new FooDeep { BarShort = 12 } }
                }
            }
        },
        new()
        {
            Foo = new FooNullable
            {
                BarShort = null,
                BarBool = null,
                BarEnum = BarEnum.BAZ,
                BarString = "testbtest",
                ObjectArray = new List<BarNullableDeep?>
                {
                    new BarNullableDeep { Foo = new FooDeep { BarShort = 9 } }
                }
            }
        },
        new()
        {
            Foo = new FooNullable
            {
                BarShort = 14,
                BarBool = false,
                BarEnum = BarEnum.QUX,
                BarString = "testctest",
                ObjectArray = new List<BarNullableDeep?>
                {
                    new BarNullableDeep { Foo = new FooDeep { BarShort = 14 } }
                }
            }
        },
        new()
        {
            Foo = new FooNullable
            {
                BarShort = 13,
                BarBool = false,
                BarEnum = BarEnum.FOO,
                BarString = "testdtest",
                ObjectArray = null
            }
        }
    };

    private static readonly BarNullable[] _barWithoutRelation =
    {
        new()
        {
            Foo = new FooNullable
            {
                BarEnum = BarEnum.BAR,
                BarShort = 15,
                NestedObject = new BarNullableDeep
                {
                    Foo = new FooDeep
                    {
                        BarString = "Foo"
                    }
                }
            }
        },
        new()
        {
            Foo = new FooNullable
            {
                BarEnum = BarEnum.FOO,
                BarShort = 14
            }
        },
        new()
    };

    private readonly SchemaCache _cache = new();

    [Fact]
    public async Task Create_DeepFilterObjectTwoProjections()
    {
        // arrange
        var tester = _cache.CreateSchema(_barEntities, OnModelCreating);

        // act
        var res1 = await tester.ExecuteAsync(
            QueryRequestBuilder.New()
                .SetQuery(
                    @"{
                        root {
                            foo {
                                objectArray(
                                    where: {
                                        foo: {
                                            barString: {
                                                eq: ""a""
                                            }
                                        }
                                    }) {
                                    foo {
                                        barString
                                        barShort
                                    }
                                }
                            }
                        }
                    }")
                .Create());

        // assert
        await SnapshotExtensions.Add(
                Snapshot
                    .Create(), res1)
            .MatchAsync();
    }

    [Fact]
    public async Task Create_ListObjectDifferentLevelProjection()
    {
        // arrange
        var tester = _cache.CreateSchema(_barEntities, OnModelCreating);

        // act
        var res1 = await tester.ExecuteAsync(
            QueryRequestBuilder.New()
                .SetQuery(
                    @"
                        {
                            root {
                                foo {
                                    barString
                                    objectArray(
                                        where: {
                                            foo: {
                                                barString: {
                                                    eq: ""a""
                                                }
                                            }
                                        }) {
                                        foo {
                                            barString
                                            barShort
                                        }
                                    }
                                }
                            }
                        }")
                .Create());

        // assert
        await SnapshotExtensions.Add(
                Snapshot
                    .Create(), res1)
            .MatchAsync();
    }

    [Fact]
    public async Task Create_DeepFilterObjectTwoProjections_Nullable()
    {
        // arrange
        var tester = _cache.CreateSchema(_barNullableEntities, OnModelCreating);

        // act
        var res1 = await tester.ExecuteAsync(
            QueryRequestBuilder.New()
                .SetQuery(
                    @"{
                        root {
                            foo {
                                objectArray(
                                    where: {
                                        foo: {
                                            barString: {
                                                eq: ""a""
                                            }
                                        }
                                    }) {
                                    foo {
                                        barString
                                        barShort
                                    }
                                }
                            }
                        }
                    }")
                .Create());

        // assert
        await SnapshotExtensions.Add(
                Snapshot
                    .Create(), res1)
            .MatchAsync();
    }

    [Fact]
    public async Task Create_ListObjectDifferentLevelProjection_Nullable()
    {
        // arrange
        var tester = _cache.CreateSchema(_barNullableEntities, OnModelCreating);

        // act
        var res1 = await tester.ExecuteAsync(
            QueryRequestBuilder.New()
                .SetQuery(
                    @"{
                        root {
                            foo {
                                barString
                                objectArray(
                                    where: {
                                        foo: {
                                            barString: {
                                                eq: ""a""
                                            }
                                        }
                                    }) {
                                    foo {
                                        barString
                                        barShort
                                    }
                                }
                            }
                        }
                    }")
                .Create());

        // assert
        await SnapshotExtensions.Add(
                Snapshot
                    .Create(), res1)
            .MatchAsync();
    }

    [Fact]
    public async Task Should_NotInitializeObject_When_ResultOfLeftJoinIsNull()
    {
        // arrange
        var tester = _cache.CreateSchema(_barWithoutRelation, OnModelCreating);

        // act
        var res1 = await tester.ExecuteAsync(
            QueryRequestBuilder.New()
                .SetQuery(
                    @"{
                        root {
                            foo {
                                id
                            }
                        }
                    }")
                .Create());

        // assert
        await SnapshotExtensions.Add(
                Snapshot
                    .Create(), res1)
            .MatchAsync();
    }

    [Fact]
    public async Task Should_NotInitializeObject_When_ResultOfLeftJoinIsNull_TwoFields()
    {
        // arrange
        var tester = _cache.CreateSchema(_barWithoutRelation, OnModelCreating);

        // act
        var res1 = await tester.ExecuteAsync(
            QueryRequestBuilder.New()
                .SetQuery(
                    @"{
                        root {
                            id
                            foo {
                                id
                                barEnum
                            }
                        }
                    }")
                .Create());

        // assert
        await SnapshotExtensions.Add(
                Snapshot
                    .Create(), res1)
            .MatchAsync();
    }

    [Fact]
    public async Task Should_NotInitializeObject_When_ResultOfLeftJoinIsNull_Deep()
    {
        // arrange
        var tester = _cache.CreateSchema(_barWithoutRelation, OnModelCreating);

        // act
        var res1 = await tester.ExecuteAsync(
            QueryRequestBuilder.New()
                .SetQuery(
                    @"{
                        root {
                            id
                            foo {
                                id
                                barEnum
                                nestedObject {
                                    foo {
                                        barString
                                    }
                                }
                            }
                        }
                    }")
                .Create());

        // assert
        await SnapshotExtensions.Add(
                Snapshot
                    .Create(), res1)
            .MatchAsync();
    }

    private static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Foo>().HasMany(x => x.ObjectArray);
        modelBuilder.Entity<Foo>().HasOne(x => x.NestedObject);
        modelBuilder.Entity<Bar>().HasOne(x => x.Foo);
    }

    public class Foo
    {
        public int Id { get; set; }

        public short BarShort { get; set; }

        public string BarString { get; set; } = string.Empty;

        public BarEnum BarEnum { get; set; }

        public bool BarBool { get; set; }

        [UseFiltering]
        public List<BarDeep>? ObjectArray { get; set; }

        public BarDeep? NestedObject { get; set; }
    }

    public class FooDeep
    {
        public int Id { get; set; }

        public short BarShort { get; set; }

        public string BarString { get; set; } = string.Empty;
    }

    public class FooNullable
    {
        public int Id { get; set; }

        public short? BarShort { get; set; }

        public string? BarString { get; set; }

        public BarEnum? BarEnum { get; set; }

        public bool? BarBool { get; set; }

        [UseFiltering]
        [UseSorting]
        public List<BarNullableDeep?>? ObjectArray { get; set; }

        public BarNullableDeep? NestedObject { get; set; }
    }

    public class Bar
    {
        public int Id { get; set; }

        public Foo Foo { get; set; } = default!;
    }

    public class BarDeep
    {
        public int Id { get; set; }

        public FooDeep Foo { get; set; } = default!;
    }

    public class BarNullableDeep
    {
        public int Id { get; set; }

        public FooDeep? Foo { get; set; }
    }

    public class BarNullable
    {
        public int Id { get; set; }

        public FooNullable? Foo { get; set; }
    }

    public enum BarEnum
    {
        FOO,
        BAR,
        BAZ,
        QUX
    }
}
