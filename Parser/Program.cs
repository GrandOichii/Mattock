using System.Text.Json;
using Parser;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var card = new Card {
    Name = "Ramunap Ruins",
    Text = "{T}: You gain 1 life.",
};

var todo = new Matcher() {
    Name = "TODO",
    PatternString = ".+",
    Script = "function _Create(children) return '-- TODO' end"
};

var manaPip = new Matcher() {
    Name = "mana-pip",
    Script = File.ReadAllText("mana-pip.lua"),
    PatternString = @"[W|U|B|R|G|C]",
};

var genericPip = new Matcher() {
    Name = "generic-pip",
    PatternString = @"[1-9]?[0-9]",
    Script = File.ReadAllText("generic-pip.lua")
};

var tap = new Matcher() {
    Name = "tap",
    Script = File.ReadAllText("tap.lua"),
    PatternString = @"\{T\}"
};

var pipSelector = new Selector() {
    Name = "pip-selector",
    Children = {
        manaPip,
        genericPip
    }
};

var pipSplitter = new Splitter() {
    Name = "pip-splitter",
    PatternString = @"\{|\}\{|\}",
    Script = File.ReadAllText("pip-splitter.lua"),
    Children = {
        pipSelector
    }
};

var payLifeCost = new Matcher() {
    Name = "pay-life-cost",
    PatternString = "pay ([0-9]+) life",
    Script = File.ReadAllText("pay-life-cost.lua")
};

var pipCost = new Matcher() {
    Name = "pip-cost",
    PatternString = @"\{(.+)\}",
    Script = File.ReadAllText("pip-cost.lua"),
    Children = {
        pipSplitter
    }
};

var costSelector = new Selector() {
    Name = "cost-selector",
    Children = {
        pipCost,
        tap,
        payLifeCost
    }
};

var cost = new Splitter() {
    Name = "cost-splitter",
    PatternString = ", ",
    Script = File.ReadAllText("cost-splitter.lua"),
    Children = {
        costSelector
    }
};

var addManaSplitter = new Splitter() {
    Name = "add-mana-splitter",
    PatternString = @" or |, or |, ",
    Script = File.ReadAllText("add-mana-splitter.lua"),
    Children = {
        // TODO not mana pip
        pipSplitter
    }
};

var addMana = new Matcher() {
    Name = "add-mana",
    PatternString = @"add (.+)\.",
    Script = File.ReadAllText("add-mana.lua"),
    Children = {
        addManaSplitter,
    }
};

var lifeGainEffect = new Matcher() {
    Name = "life-gain-effect",
    PatternString = "you gain ([0-9]+) life.",
    Script = File.ReadAllText("life-gain.lua")
};

var effect = new Selector() {
    Name = "effect",
    Script = File.ReadAllText("effect.lua"),
    Children = {
        addMana,
        lifeGainEffect
        // todo
    }
};

var activatedAbility = new Matcher() {
    Name = "activated-ability",
    PatternString = "(.+): (.+)",
    Script = File.ReadAllText("activated-ability.lua"),
    Children = {
        cost,
        effect
    }
};


var parser = new Matcher(){
    Name = "root",
    PatternString = "(.+)",
    Script = File.ReadAllText("root.lua"),
    Children = {
        activatedAbility
    }
};


var result = parser.Parse(card.Text);

var serializer = new SerializerBuilder()
    .WithNamingConvention(CamelCaseNamingConvention.Instance)
    .Build();
var serialized = serializer.Serialize(result);

var script = result.CreateScript();

File.WriteAllText("result.lua", script);

File.WriteAllText("result.yaml", serialized);