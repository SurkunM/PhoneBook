<template>
    <v-dialog v-model="isShow" persistent max-width="600px">
        <v-card>
            <v-toolbar dark color="primary">
                <v-toolbar-title>Редактирование контакта</v-toolbar-title>

                <v-btn icon dark @click="hide">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>

            <v-form ref="form">
                <v-card-text>
                    <v-text-field v-model.trim="editedContact.firstName"
                                  label="Имя"
                                  :error-messages="errors.firstName">
                    </v-text-field>

                    <v-text-field v-model.trim="editedContact.lastName"
                                  label="Фамилия"
                                  :error-messages="errors.lastName">
                    </v-text-field>

                    <v-text-field v-model.trim="editedContact.phone"
                                  label="Телефон"
                                  :error-messages="errors.phone">
                    </v-text-field>
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue lighten-1" text @click="$emit('saved', editedContact)">Сохранить</v-btn>
                    <v-btn color="error" text @click="hide">Отменить</v-btn>
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
                editedContact: null,

                errors: {
                    firstName: "",
                    lastName: "",
                    phone: ""
                }
            };
        },

        methods: {
            checkEditingFirstName(firstName) {
                if (firstName?.length > 0) {
                    return true;
                }

                return "Заполните поле Имя";
            },

            checkEditingLastName(lastName) {
                if (lastName?.length >= 2) {
                    return true;
                }

                return "Заполните поле Фамилия";
            },

           checkEditingPhone(phone) {
                if (/^[0-9-]{7,}$/.test(phone)) {
                    return true;
                }

                return "Неверный формат для поля телефон";
            },

            submitForm() {
                this.errors.firstName = this.checkEditingFirstName(this.editedContact.firstName)
                this.errors.lastName = this.checkEditingLastName(this.editedContact.lastName)
                this.errors.phone = this.checkEditingPhone(this.editedContact.phone)
            },

            resetErrors() {
                this.errors = {
                    firstName: "",
                    lastName: "",
                    phone: ""
                }
            },

            show(contact) {
                this.resetErrors();
                this.editedContact = contact;

                this.isShow = true;
            },

            hide() {
                this.isShow = false;
            }
        },

        emits: ["saved"]
    }
</script>