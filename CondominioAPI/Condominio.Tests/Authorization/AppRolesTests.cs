using NUnit.Framework;
using Condominio.Utils.Authorization;

namespace Condominio.Tests.Authorization
{
    [TestFixture]
    public class AppRolesTests
    {
        [Test]
        public void Super_HasCorrectValue()
        {
            Assert.AreEqual("super", AppRoles.Super);
        }

        [Test]
        public void Administrador_HasCorrectValue()
        {
            Assert.AreEqual("admin", AppRoles.Administrador);
        }

        [Test]
        public void Director_HasCorrectValue()
        {
            Assert.AreEqual("director", AppRoles.Director);
        }

        [Test]
        public void Habitante_HasCorrectValue()
        {
            Assert.AreEqual("habitante", AppRoles.Habitante);
        }

        [Test]
        public void Auxiliar_HasCorrectValue()
        {
            Assert.AreEqual("auxiliar", AppRoles.Auxiliar);
        }

        [Test]
        public void Seguridad_HasCorrectValue()
        {
            Assert.AreEqual("seguridad", AppRoles.Seguridad);
        }

        [Test]
        public void AllRoles_ContainsAllRoles()
        {
            var allRoles = AppRoles.AllRoles;
            Assert.IsNotNull(allRoles);
            Assert.AreEqual(6, allRoles.Length);
        }

        [Test]
        public void AllRoles_ContainsSuper()
        {
            Assert.Contains(AppRoles.Super, AppRoles.AllRoles);
        }

        [Test]
        public void AllRoles_ContainsAdministrador()
        {
            Assert.Contains(AppRoles.Administrador, AppRoles.AllRoles);
        }

        [Test]
        public void AllRoles_ContainsDirector()
        {
            Assert.Contains(AppRoles.Director, AppRoles.AllRoles);
        }

        [Test]
        public void AllRoles_ContainsHabitante()
        {
            Assert.Contains(AppRoles.Habitante, AppRoles.AllRoles);
        }

        [Test]
        public void AllRoles_ContainsAuxiliar()
        {
            Assert.Contains(AppRoles.Auxiliar, AppRoles.AllRoles);
        }

        [Test]
        public void AllRoles_ContainsSeguridad()
        {
            Assert.Contains(AppRoles.Seguridad, AppRoles.AllRoles);
        }

        [Test]
        public void AllRoles_HasCorrectOrder()
        {
            var allRoles = AppRoles.AllRoles;
            Assert.AreEqual(AppRoles.Super, allRoles[0]);
            Assert.AreEqual(AppRoles.Administrador, allRoles[1]);
            Assert.AreEqual(AppRoles.Director, allRoles[2]);
            Assert.AreEqual(AppRoles.Habitante, allRoles[3]);
            Assert.AreEqual(AppRoles.Auxiliar, allRoles[4]);
            Assert.AreEqual(AppRoles.Seguridad, allRoles[5]);
        }

        [Test]
        public void AllRoles_AreNotEmpty()
        {
            var allRoles = AppRoles.AllRoles;
            foreach (var role in allRoles)
            {
                Assert.IsNotEmpty(role);
            }
        }

        [Test]
        public void AdminRoles_ContainsOnlyAdminRoles()
        {
            var adminRoles = AppRoles.AdminRoles;
            Assert.IsNotNull(adminRoles);
            Assert.AreEqual(2, adminRoles.Length);
        }

        [Test]
        public void AdminRoles_ContainsSuper()
        {
            Assert.Contains(AppRoles.Super, AppRoles.AdminRoles);
        }

        [Test]
        public void AdminRoles_ContainsAdministrador()
        {
            Assert.Contains(AppRoles.Administrador, AppRoles.AdminRoles);
        }

        [Test]
        public void AdminRoles_DoesNotContainDirector()
        {
            Assert.IsFalse(AppRoles.AdminRoles.Contains(AppRoles.Director));
        }

        [Test]
        public void AdminRoles_DoesNotContainHabitante()
        {
            Assert.IsFalse(AppRoles.AdminRoles.Contains(AppRoles.Habitante));
        }

        [Test]
        public void AdminRoles_DoesNotContainAuxiliar()
        {
            Assert.IsFalse(AppRoles.AdminRoles.Contains(AppRoles.Auxiliar));
        }

        [Test]
        public void AdminRoles_DoesNotContainSeguridad()
        {
            Assert.IsFalse(AppRoles.AdminRoles.Contains(AppRoles.Seguridad));
        }

        [Test]
        public void AdminRoles_HasCorrectOrder()
        {
            var adminRoles = AppRoles.AdminRoles;
            Assert.AreEqual(AppRoles.Super, adminRoles[0]);
            Assert.AreEqual(AppRoles.Administrador, adminRoles[1]);
        }

        [Test]
        public void RoleValuesAreUnique()
        {
            var allRoles = AppRoles.AllRoles;
            var uniqueRoles = new HashSet<string>(allRoles);
            Assert.AreEqual(allRoles.Length, uniqueRoles.Count);
        }

        [Test]
        public void AllRoles_AreNotNull()
        {
            var allRoles = AppRoles.AllRoles;
            Assert.IsNotNull(allRoles);
            foreach (var role in allRoles)
            {
                Assert.IsNotNull(role);
            }
        }

        [Test]
        public void AdminRoles_AreNotNull()
        {
            var adminRoles = AppRoles.AdminRoles;
            Assert.IsNotNull(adminRoles);
            foreach (var role in adminRoles)
            {
                Assert.IsNotNull(role);
            }
        }

        [Test]
        public void AllRoles_CanBeIteratedMultipleTimes()
        {
            var allRoles = AppRoles.AllRoles;
            var firstIteration = string.Join(",", allRoles);
            var secondIteration = string.Join(",", allRoles);
            Assert.AreEqual(firstIteration, secondIteration);
        }

        [Test]
        public void AdminRoles_CanBeIteratedMultipleTimes()
        {
            var adminRoles = AppRoles.AdminRoles;
            var firstIteration = string.Join(",", adminRoles);
            var secondIteration = string.Join(",", adminRoles);
            Assert.AreEqual(firstIteration, secondIteration);
        }
    }
}
