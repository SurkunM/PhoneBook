<template>
    <v-card-title class="bg-grey-darken-1 ">
        <h2 class="me-4">
            <v-icon icon="mdi-account-plus" size="small"></v-icon>
            Новый контакт
        </h2>
    </v-card-title>

    <v-alert text="Контакт успешно создан" type="success" variant="outlined" v-show="isShowSuccessAlert"></v-alert>

    <form @submit.prevent="submitForm">
        <v-text-field v-model.trim="contact.lastName"
                      :error-messages="errors.lastName"
                      @change="checkLastNameFieldComplete"
                      autocomplete="off"
                      label="Фамилия">
        </v-text-field>

        <v-text-field v-model.trim="contact.firstName"
                      :error-messages="errors.firstName"
                      @change="checkFirstNameFieldComplete"
                      autocomplete="off"
                      label="Имя">
        </v-text-field>

        <v-text-field v-model.trim="contact.phone"
                      :error-messages="errors.phone"
                      @change="checkPhoneFieldComplete"
                      autocomplete="off"
                      label="Телефон"
                      class="mb-3">
        </v-text-field>

        <v-btn class="me-4" color="info" type="submit">Сохранить</v-btn>
        <v-btn color="secondary" @click="resetForm">Очистить</v-btn>
    </form>
</template>

<script>
    export default {
        data() {
            return {
                isShowSuccessAlert: false,

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

                const phoneNumber = Number(contact.phone);

                if (isNaN(phoneNumber) || phoneNumber < 0) {
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

            setExistPhoneInvalid() {
                this.errors.phone = "Номер телефона уже существует";
            },

            submitForm() {
                if (!this.checkFieldsIsvalid(this.contact)) {
                    return;
                }

                const createdContact = {
                    firstName: this.contact.firstName,
                    lastName: this.contact.lastName,
                    phone: this.contact.phone
                };

                this.$store.dispatch("createContact", createdContact)
                    .then(() => {
                        this.resetForm();
                        this.showSuccessAlert();
                    })
                    .catch(error => {
                        if (error.response?.status === 400) {
                            this.checkFieldsIsvalid(createdContact);
                        }
                        else if (error.response?.status === 409) {
                            this.setExistPhoneInvalid();
                        }
                    });
            },

            resetForm() {
                this.contact = {
                    firstName: "",
                    lastName: "",
                    phone: ""
                };

                this.errors = {
                    firstName: "",
                    lastName: "",
                    phone: ""
                };
            },

            showSuccessAlert() {
                this.isShowSuccessAlert = true;

                setTimeout(() => {
                    this.isShowSuccessAlert = false;
                }, 1500);
            }
        }
    }
</script>