<template>
    <v-card-title class="bg-grey-darken-1 ">
        <h2 class="me-4">
            <v-icon icon="mdi-account-plus" size="small"></v-icon>
            Новый контакт
        </h2>
    </v-card-title>

    <form @submit.prevent="submitForm">
        <v-text-field v-model.trim="contact.firstName"
                      :error-messages="errors.firstName"
                      @change="checkFirstNameFieldComplete"
                      label="Имя">
        </v-text-field>

        <v-text-field v-model.trim="contact.lastName"
                      :error-messages="errors.lastName"
                      @change="checkLastNameFieldComplete"
                      label="Фамилия">
        </v-text-field>

        <v-text-field v-model.trim="contact.phone"
                      :error-messages="errors.phone"
                      @change="checkPhoneFieldComplete"
                      label="Телефон">
        </v-text-field>

        <v-btn class="me-4" type="submit">Сохранить</v-btn>

        <v-btn @click="resetForm">Очистить</v-btn>
    </form>
</template>

<script>
    export default {
        data() {
            return {
                contact: {
                    firstName: "",
                    lastName: "",
                    phone: "",
                    isChecked: false
                },

                errors: {
                    firstName: "",
                    lastName: "",
                    phone: ""
                }
            }
        },

        methods: {
            checkFieldsIsvalid(contact) {
                let isValid = true;

                if (contact.firstName.length === 0) {
                    this.errors.firstName = "Заполните поле Имя";
                    isValid = false;
                }

                if (contact.lastName.length === 0) {
                    this.errors.lastName = "Заполните поле Фамилия";
                    isValid = false;
                }

                if (contact.phone.length === 0) {
                    this.errors.phone = "Заполните поле Телефон";
                    isValid = false;
                }

                if (isNaN(Number(contact.phone))) {
                    this.errors.phone = "Неверный формат номера телефона";
                    isValid = false;
                }

                return isValid;
            },

            checkFirstNameFieldComplete() {
                if (this.contact.firstName.length > 0) {
                    this.errors.firstName = "";
                }
            },

            checkLastNameFieldComplete() {
                if (this.contact.lastName.length > 0) {
                    this.errors.lastName = "";
                }
            },

            checkPhoneFieldComplete() {
                if (this.contact.phone.length > 0) {
                    this.errors.phone = "";
                }
            },

            submitForm() {
                //if (!this.checkFieldsIsvalid(this.contact)) {
                //    return;
                //}

                const createdContact = {
                    firstName: this.contact.firstName,
                    lastName: this.contact.lastName,
                    phone: this.contact.phone
                };

                this.$store.dispatch("createContact", createdContact)
                    .then(() => {
                        alert("Контакт успешно создан!");
                        this.resetForm();
                    })
                    .catch(() => {
                        this.checkFieldsIsvalid(createdContact);
                        alert("Ошибка! Не удалось создать контакт.");  //TODO: 3. Проверить то что такой телефон уже сущ.
                    });
            },            

            resetForm() {
                this.contact = {
                    firstName: "",
                    lastName: "",
                    phone: ""
                },

                    this.errors = {
                        firstName: "",
                        lastName: "",
                        phone: ""
                    }
            }
        }
    }
</script>