using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// In SDK-style projects such as this one, several assembly attributes that were historically
// defined in this file are now automatically added during build and populated with
// values defined in project properties. For details of which attributes are included
// and how to customize this process see: https://aka.ms/assembly-info-properties

// Setting ComVisible to false makes the types in this assembly not visible to COM
// components.  If you need to access a type in this assembly from COM, set the ComVisible
// attribute to true on that type.
[assembly: ComVisible(false)]

// Identify the assembly CLS-compliance level
[assembly: CLSCompliant(true)]

// InternalsVisibleTo attribute is used to specify that the internal types of this assembly
// are visible to another assembly. This is often used for unit testing purposes.
// The specified assembly name must match the name of the assembly that will access the internal types.
// See https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.internalsvisibletoattribute
[assembly: InternalsVisibleTo("FluentHttpClient.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100e1adf9793254ed4e9a9068dbcbb2d9f062ca131bca90fedd7b1f0b5aecb4a6561d32f674c3c459b3aa910a43bede8e8ac5953e03ac29209aec1f3b8d5a112382ad517e3dacc2872cb8444552bd70a41420e93ddfd75b208ab2af3a11bddf5ecdf1e26f0a8f9d0e58d45ec359e3debedb55fa62e66f190e21995be769ba67aeae")]
