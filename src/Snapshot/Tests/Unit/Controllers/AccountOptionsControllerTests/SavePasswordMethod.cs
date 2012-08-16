using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Models.AccountOptions;
using Web.Models.Shared;
using Core.Domain;

namespace Tests.Unit.Controllers.AccountOptionsControllerTests
{
    [TestFixture]
    public class SavePasswordMethod
    {
        public ObjectMother objectMother = new ObjectMother();
        
        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void IfCurrentPassword_IsNotTheSame_ItShouldReturn_JsonWithErrorMessage()
        {
            //Arange
            PasswordModel model = new PasswordModel { CurrentPassword = "SomePassword" };
            objectMother.queryUsers.Expect(call => call.Load(objectMother.userId)).Return(objectMother.user);
            objectMother.queryUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());
            objectMother.queryClient.Expect(call => call.Load(objectMother.clientId)).Return(objectMother.client);

            //Act
            var result = objectMother.controller.SavePassword(model);

            //Assert
            objectMother.queryUsers.VerifyAllExpectations();
            objectMother.queryClient.VerifyAllExpectations();

            var response = result.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }

        [Test]
        public void IfValidationIsCorrect_ItShouldSaveTheUserNewPassword()
        {
            //Arange
            PasswordModel model = new PasswordModel { 
                CurrentPassword = objectMother.user.Password,
                NewPassword = "NewPassword"
            };
            objectMother.queryUsers.Expect(call => call.Load(objectMother.userId)).Return(objectMother.user);
            objectMother.queryUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());
            objectMother.queryClient.Expect(call => call.Load(objectMother.clientId)).Return(objectMother.client);
            objectMother.securePassword.Expect(call => call.EncryptPassword(model.NewPassword)).Return(model.NewPassword);
            objectMother.securePassword.Expect(call => call.EncryptPassword(model.CurrentPassword)).Return(objectMother.user.Password);

            objectMother.saveCommand.Expect(call => call.Execute(Arg<User>.Matches(p => p.FirstName == objectMother.user.FirstName &&
                                                                                        p.LastName == objectMother.user.LastName &&
                                                                                        p.Email == objectMother.user.Email &&
                                                                                        p.Password == model.NewPassword
                                                                                   )));

            //Act
            var result = objectMother.controller.SavePassword(model);

            //Assert
            objectMother.queryUsers.VerifyAllExpectations();
            objectMother.queryClient.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();
            objectMother.securePassword.VerifyAllExpectations();

            var response = result.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
        }
    }
}
