<template>
    <tr>     
        <td>
            <v-checkbox hide-details></v-checkbox>
        </td>
        
        <td>{{ index + 1 }}</td>
        
        <td>{{ contact.lastName }}</td>
        
        <td>{{ contact.firstName }}</td>
        
        <td>{{ contact.phone }}</td>
        
        <td>
            <v-btn small color="info" @click="$emit('edit', (contact))" class="me-2">Редактировать</v-btn>
            <v-btn small color="error" @click="$emit('delete', (contact))">Удалить</v-btn>
        </td>
    </tr>
</template>

<script>
    export default {
        props: {
            contact: {
                type: Object,
                required: true
            },
            index: {
                type: Number,
                required: true
            }
        },

        emits: ["edit", "delete"]
    }
</script>