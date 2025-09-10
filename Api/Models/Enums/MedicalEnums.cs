namespace Api.Models.Enums
{
    public enum MedicalCondition
    {
        None,
        Asthma,
        Diabetes,
        Hypertension,
        HeartDisease,
        Arthritis,
        Obesity,
        HighCholesterol,
        BackPain,
        Pregnancy,
        Epilepsy,
        RespiratoryIssues,
        JointProblems,
        Other
    }


    public enum MedicationType
    {
        None,
        BloodPressure,
        Cholesterol,
        Diabetes,
        Asthma,
        PainRelief,
        AntiInflammatory,
        Antidepressant,
        BirthControl,
        Thyroid,
        Allergy,
        Heart,
        Other
    }
    public enum SurgeryType
    {
        None,
        Cardiac,
        Orthopedic,
        Neurological,
        Gastrointestinal,
        Cosmetic,
        Gynecological,
        Urological,
        Respiratory,
        Ophthalmological,
        ENT, // Ear, Nose, Throat
        Other
    }

    public enum MuscleGroup
    {
        None,
        Chest,
        Back,
        Shoulders,
        Arms,
        Legs,
        Core,
        Glutes,
        Calves,
        Forearms,
        Neck,
        FullBody,
        Other
    }

    public enum ChestMuscleGroup
    {
        None,
        UpperChest,
        MiddleChest,
        LowerChest,
        InnerChest,
        OuterChest,
        SerratusAnterior,
        PectoralisMajor,
        PectoralisMinor,
        Other
    }

    public enum BackMuscleGroup
    {
        None,
        UpperBack,
        MiddleBack,
        LowerBack,
        LatissimusDorsi,
        Trapezius,
        Rhomboids,
        TeresMajor,
        ErectorSpinae,
        Infraspinatus,
        Other
    }

    public enum ShoulderMuscleGroup
    {
        None,
        AnteriorDeltoid,
        LateralDeltoid,
        PosteriorDeltoid,
        RotatorCuff,
        Supraspinatus,
        Subscapularis,
        Other
    }

    public enum ArmMuscleGroup
    {
        None,
        Biceps,
        Triceps,
        Brachialis,
        Brachioradialis,
        Forearms,
        Anconeus,
        Other
    }

    public enum LegMuscleGroup
    {
        None,
        Quadriceps,
        Hamstrings,
        Glutes,
        Adductors,
        Abductors,
        Calves,
        TibialisAnterior,
        Sartorius,
        HipFlexors,
        Other
    }

    public enum CoreMuscleGroup
    {
        None,
        RectusAbdominis,
        Obliques,
        TransverseAbdominis,
        Multifidus,
        QuadratusLumborum,
        PelvicFloor,
        Diaphragm,
        HipFlexors,
        Other
    }
}



