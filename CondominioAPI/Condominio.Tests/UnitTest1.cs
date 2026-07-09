namespace Condominio.Tests
{
    /// <summary>
    /// General test fixtures for Condominio.Utils project
    /// Individual test classes are organized by module:
    /// - Utils: DTOConverterTests.cs, PasswordHasherTests.cs
    /// - Authorization: AppRolesTests.cs
    /// - Enums: PaymentStatusEnumTests.cs
    /// </summary>
    [TestFixture]
    public class CondinioUtilsGeneralTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CondinioUtils_ProjectLoads_Successfully()
        {
            // This test verifies the Condominio.Utils project loads correctly
            var utilsType = typeof(Condominio.Utils.PasswordHasher);
            Assert.IsNotNull(utilsType);
            Assert.AreEqual("Condominio.Utils", utilsType.Namespace);
        }

        [Test]
        public void DTOConverter_Exists()
        {
            var dtoConverterType = typeof(Condominio.Utils.DTOConverter);
            Assert.IsNotNull(dtoConverterType);
        }

        [Test]
        public void AppRoles_Exists()
        {
            var appRolesType = typeof(Condominio.Utils.Authorization.AppRoles);
            Assert.IsNotNull(appRolesType);
        }

        [Test]
        public void PaymentStatusEnum_Exists()
        {
            var enumType = typeof(Condominio.Utils.Enums.PaymentStatus);
            Assert.IsNotNull(enumType);
            Assert.IsTrue(enumType.IsEnum);
        }
    }
}