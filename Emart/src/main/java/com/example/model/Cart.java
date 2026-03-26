package com.example.model;

import java.math.BigDecimal;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;

@Entity
@Table(name="Cart")
public class Cart {

	@Id
	@GeneratedValue(strategy=GenerationType.IDENTITY)
	@Column(name="Cart_Id")
	private int cartId;
	
	@ManyToOne(fetch =FetchType.LAZY)
	@JoinColumn(name="User_Id")
	private int Customer customer;
	
	@Column(name="Total_Amount", precision=10,scale=2)
	private BigDecimal totalAmount;
	
}
