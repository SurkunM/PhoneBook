<template>
    <v-dialog v-model="isShow" persistent max-width="600px">
        <v-card>
            <v-toolbar dark color="primary">
                <v-toolbar-title>Редактирование контакта</v-toolbar-title>

                <v-btn icon dark @click="hide">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>

            <v-form @submit.prevent="submitForm">
                <v-card-text>
                    <v-text-field v-model.trim="editedContact.lastName"
                                  label="Фамилия"
                                  :error-messages="errors.lastName"
                                  autocomplete="off"
                                  @change="checkLastNameFieldComplete">
                    </v-text-field>

                    <v-text-field v-model.trim="editedContact.firstName"
                                  label="Имя"
                                  :error-messages="errors.firstName"
                                  autocomplete="off"
                                  @change="checkFirstNameFieldComplete">
                    </v-text-field>

                    <v-text-field v-model.trim="editedContact.phone"
                                  label="Телефон"
                                  :error-messages="errors.phone"
                                  autocomplete="off"
                                  @change="checkPhoneFieldComplete">
                    </v-text-field>
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="info" type="submit">Сохранить</v-btn>
                    <v-btn color="error" @click="hide">Отменить</v-btn>
                </v-card-actions>
            </v-form>
        </v-card>
    </v-dialog>
</template>

<script>
    export default {
        data() {
            return {
                isShow: false,

                editedContact: {
                    id: 0,
                    firstName: "",
                    lastName: "",
                    phone: ""
                },

                errors: {
                    firstName: "",
                    lastName: "",
                    phone: ""
                }
            };
        },

        methods: {
            checkEditingFieldsIsvalid(contact) {
                this.resetErrors();
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
                if (this.editedContact.firstName.length > 0) {
                    this.errors.firstName = "";
                }
            },

            checkLastNameFieldComplete() {
                if (this.editedContact.lastName.length > 0) {
                    this.errors.lastName = "";
                }
            },

            checkPhoneFieldComplete() {
                if (this.editedContact.phone.length > 0) {
                    this.errors.phone = "";
                }
            },

            setExistPhoneInvalid() {
                this.errors.phone = "Номер телефона уже существует";
            },

            submitForm() {
                if (!this.checkEditingFieldsIsvalid(this.editedContact)) {
                    return;
                }

                this.$emit('save', this.editedContact);
            },

            resetErrors() {
                this.errors = {
                    firstName: "",
                    lastName: "",
                    phone: ""
                };
            },

            show(contact) {
                this.resetErrors();

                this.editedContact.id = contact.id;
                this.editedContact.firstName = contact.firstName;
                this.editedContact.lastName = contact.lastName;
                this.editedContact.phone = contact.phone;

                this.isShow = true;
            },

            hide() {
                this.isShow = false;
            }
        },

        emits: ["save"]
    }
</script>