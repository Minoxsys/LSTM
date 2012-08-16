using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.AccountOptions;
using Rhino.Mocks;
using Core.Domain;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.AccountOptionsControllerTests
{
    [TestFixture]
    public class SaveProfileInformationMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_WhenProfileInformation_HasBeenSaved()
        {
            //Arrange
            UserModel model = new UserModel
            {
                FirstName = "NewName",
                LastName = "NewLastName",
                Email = "NewEmail"
            };
            objectMother.queryUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user}.AsQueryable());
            objectMother.queryUsers.Expect(call => call.Load(objectMother.userId)).Return(objectMother.user);
            objectMother.queryClient.Expect(call => call.Load(objectMother.clientId)).Return(objectMother.client);
            objectMother.saveCommand.Expect(call => call.Execute(Arg<User>.Matches(p => p.FirstName == model.FirstName &&
                                                                                        p.LastName == model.LastName &&
                                                                                        p.Email == model.Email &&
                                                                                        p.Password == objectMother.user.Password &&
                                                                                        p.UserName == objectMother.user.UserName
                                                                                   )));

            //Act
            var result = objectMother.controller.SaveProfileInformation(model);

            //Assert
            objectMother.queryUsers.VerifyAllExpectations();
            objectMother.queryClient.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();

            var response = result.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
        }
    }
}
