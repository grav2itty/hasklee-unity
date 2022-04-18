namespace Hasklee {

enum NodeT : byte
{
    DummyObj = 0,
    MeshObj = 1,
    MeshRefObj = 2,
    InstanceObj = 55
}

enum AttributeT : short
{
    NoAtr = 0,
    ColourAtr,
    AlphaColourAtr,
    SpecularAtr,
    TransformAtr,
    ColliderAtr,
    ColliderConvexAtr,
    ColliderDisableAtr,
    ComponentAtr,
    IgnoreRayCastAtr,
    CsoundInline,
    PathAtr,
    PathRefAtr,
    DanceAtr,
    DanceInstance,
    LightAtr,
    RealIDAtr,
    RealIDTAtr,
    NameAtr,
    MaterialAtr,
    TagAtr,
    ScalarAtr,
    CustomInt,
    VectorAtr,
    VectorAtr4,
    LuaCode,
    LuaController,

    ActionRRCodeString = 900,
    ActionRRCodeRef = 901,
    ActionSSCodeString = 902,
    ActionSSCodeRef = 903,
}

enum ComponentT : short
{
    GraphPropagate = 0,
    IdGraphRC,
    GraphConductor,
    Drag,
    DragC,
    DragSelf,
    Bell,
    StringPluck,
    Button,
    Key,
    Click,
    AntennaUnfold,
    Rigidbody,
    ConfigurableJointN,
    FixedJointN,
    Rotator,
    ParentRotator,
    Translator,
    Slider,
    Stop,
    CameraNode,
    Proximity,
    LuaComponent,
    CustomComponent,

    ParamC,
}

}
