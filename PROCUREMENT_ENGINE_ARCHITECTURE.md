# Procurement & Vendor Management Engine Architecture
## Supermarket / Shopping Mart System

---

## 1. Complete PO State Machine

### State Transition Diagram

```
                    +---> Draft ──→ Cancelled
                    |
Stock Reorder ─→ Material Request ─→ RFQ (optional) ─→ PO Draft
                                                              │
                                                              ▼
                                                         PO Sent
                                                              │
                                                    ┌─────────┴──────────┐
                                                    ▼                    ▼
                                              Acknowledged        Acknowledged
                                              (Accepted)          (Rejected/Flagged)
                                                    │                    │
                                                    ▼                    ▼
                                              Confirmed           Re-negotiate / Cancel
                                                    │
                                          ┌─────────┴──────────┐
                                          ▼                    ▼
                                    Partially Received    Fully Received
                                          │                    │
                                          ▼                    ▼
                                    Partially Closed     GRN Completed
                                          │                    │
                                          ▼                    ▼
                                     Backorder PO         Quality Inspect
                                          │               ┌────┴────┐
                                          ▼               ▼         ▼
                                    (re-enters          Accepted  Rejected
                                     fulfillment)          │         │
                                                           ▼         ▼
                                                      Stock In    Return to
                                                      Unrestricted  Vendor
                                                           │         │
                                                           ▼         ▼
                                                      Invoice     Debit Note
                                                      Matching    Created
                                                           │         │
                                                           ▼         ▼
                                                      3-Way Pass   3-Way Fail
                                                           │         │
                                                           ▼         ▼
                                                      Payment     Hold/Review
                                                      Scheduled
                                                           │
                                                           ▼
                                                       PO Closed
```

### State Definitions (based on SAP/Oracle/ERPNext patterns)

| State | Description | Constraints |
|-------|-------------|-------------|
| **Draft** | PO being created; editable | No stock impact; no supplier visibility |
| **Sent** | Issued to supplier via portal/email/EDI | Locked for edit; awaiting acknowledgment |
| **Acknowledged** | Supplier accepted terms | GRN can begin; partial ack = partial commitment |
| **Confirmed** | Supplier committed to delivery schedule | Shipment tracking active; ASN expected |
| **In Transit** | ASN received from supplier | Goods expected at warehouse |
| **Partially Received** | Some lines/quantities GRN-ed | Remaining qty tracked as open |
| **Fully Received** | All lines GRN-ed | Ready for 3-way matching |
| **Under Inspection** | QI in progress | Stock in quarantine/blocked |
| **Inspected — Accepted** | QI passed | Stock moves to unrestricted |
| **Inspected — Rejected** | QI failed | Stock blocked; return initiated |
| **Invoiced** | Vendor invoice received | 3-way matching active |
| **Payment Due** | 3-way match passed | Payment scheduled per terms |
| **Payment Complete** | Payment processed | Financial closure |
| **Closed** | All items received, inspected, invoiced, paid | Terminal state; read-only |
| **Cancelled** | PO voided before fulfillment | No further actions allowed |
| **Return Initiated** | Debit note created for rejected/changed items | Partial reversal active |
| **Return Complete** | Goods returned, debit note settled | PO adjusted |

### Key SAP M3 Status Codes (for reference)

| Code | Meaning |
|------|---------|
| 50 | Purchase order created |
| 55 | Purchase order released |
| 60 | Quality inspection partially performed |
| 65 | Quality inspection completed |
| 64 | Rejected after QI, course not determined |
| 69 | Rejected after QI, course determined |

---

## 2. GRN Inspection Matrix with Acceptance/Rejection Workflows

### GRN Creation Flow

```
Supplier Delivery Arrives
        │
        ▼
Gate Entry (Door Number, Vehicle, Driver, Challan)
        │
        ▼
Basic Verification (Challan vs PO: PO#, Item, Qty, Supplier)
        │
        ▼
┌──────────────────────────────────────────────────────┐
│         Item-level Routing Decision                  │
│  Is QI Required for this Item/Supplier combo?        │
│  (Defined in Item Master + Supplier Master)          │
└──────────────────┬───────────────────────────────────┘
                   │
     ┌─────────────┴─────────────┐
     ▼                           ▼
 QI Required                QI Not Required
     │                           │
     ▼                           ▼
 Goods Receipt Blocked       GRN Created (Unrestricted)
 (QI Stock / Quarantine)     Stock +ve update
     │
     ▼
 Sampling & Inspection
 (Per QI Plan / Template)
     │
     ▼
┌──────────────────────────────────────────────────────┐
│              Quality Inspection Matrix                │
├──────────────┬───────────────────┬───────────────────┤
│  Inspection  │    Criteria       │   Result           │
│  Parameter   │    (Min/Max)      │                    │
├──────────────┼───────────────────┼───────────────────┤
│ Visual       │ No damage,        │ Pass / Fail        │
│              │ correct labeling  │                    │
├──────────────┼───────────────────┼───────────────────┤
│ Dimensional  │ ± tolerance       │ Pass / Fail        │
│ (weight/size)│ per spec          │                    │
├──────────────┼───────────────────┼───────────────────┤
│ Expiry Date  │ ≥ 30 days from    │ Pass / Conditional │
│              │ receipt           │ / Fail             │
├──────────────┼───────────────────┼───────────────────┤
│ Temperature  │ Cold chain log    │ Pass / Fail        │
│ (frozen/chill)│ within limits    │                    │
├──────────────┼───────────────────┼───────────────────┤
│ Packaging    │ Intact seals,     │ Pass / Fail        │
│ Integrity    │ no leaks/tears    │                    │
├──────────────┼───────────────────┼───────────────────┤
│ Certification│ MTC/CoA matches   │ Pass / Hold / Fail │
│ Docs         │ batch & spec      │                    │
├──────────────┼───────────────────┼───────────────────┤
│ Quantity     │ ± 2% bulk /       │ Pass / Shortage /  │
│ Tolerance    │ 0% packaged       │ Overage            │
└──────────────┴───────────────────┴───────────────────┘
                   │
                   ▼
┌──────────────────────────────────────────────────────┐
│              Usage Decision (UD)                      │
├──────────────┬───────────────────────────────────────┤
│ UD Code      │ Action                                 │
├──────────────┼───────────────────────────────────────┤
│ A (Accept)   │ Stock → Unrestricted (SAP MT 321)      │
│ A-Minor      │ Accept with remarks; auto-notify       │
│ R (Reject)   │ Stock → Blocked; Return to Vendor      │
│ R-Scrap      │ Stock → Scrap (perishable damage)      │
│ Hold         │ Stock → Quarantine; pending review      │
│ Partial      │ Split stock: % accept, % reject        │
└──────────────┴───────────────────────────────────────┘
```

### GRN Acceptance/Rejection Decision Matrix

| Scenario | QI Result | Quantity Match | Action |
|----------|-----------|----------------|--------|
| All OK | Pass | Exact | Accept full; unrestricted stock |
| Pass with notes | Pass with remarks | Exact | Accept; flag supplier in scorecard |
| Partial damage | Some items fail | N/A | Accept good qty; reject damaged; debit note for damaged |
| Short delivery | Pass (or N/A) | < PO qty | Accept received; mark PO partially received; backorder if needed |
| Over delivery | Pass (or N/A) | > PO qty | Accept up to PO qty + tolerance; return excess |
| Wrong item | N/A (wrong SKU) | N/A | Full rejection; return to vendor |
| Quality fail | Fail | Exact | Full rejection; return to vendor; debit note |
| Expired/short expiry | Fail | Exact | Reject if < threshold; conditional accept with discount |
| Missing docs | N/A | Exact | Hold in quarantine; release when docs received |

### Debit Note Workflow (Purchase Returns)

```
Rejected / Return Goods
        │
        ▼
Create Return Request (linked to original GRN line)
        │
        ▼
Warehouse picks & packs return goods
        │
        ▼
Goods Issue posted (inventory decrements)
        │
        ▼
Debit Note Created (financial document)
   - References original PO & GRN
   - Amount = Qty × Unit Price (or negotiated)
   - Includes tax adjustment
        │
        ▼
┌──────────────────────────────────────────────────────┐
│  Debit Note States (O2VEND/Odoo Pattern)             │
├──────────────────────────────────────────────────────┤
│ New Return → Approved → Goods Returned → Closed      │
│      ↘ Rejected ↘ Return Canceled                    │
└──────────────────────────────────────────────────────┘
        │
        ▼
Debit Note matched against Supplier Credit Note
        │
        ▼
Payment adjusted (reduced by debit note amount)
        │
        ▼
PO line adjusted (received qty reduced by return qty)
```

---

## 3. Three-Way Matching Algorithm

### Core Concept

Three-way matching verifies that the **Purchase Order (PO)**, **Goods Receipt Note (GRN)**, and **Vendor Invoice** are consistent before releasing payment.

### Matching Algorithm (Pseudocode)

```
FUNCTION ThreeWayMatch(po_line, grn_line, invoice_line):
    match_result = {status: "PASS", flags: [], discrepancies: []}
    
    // ─────────────────────────────────────────────
    // LEVEL 1: QUANTITY MATCH (PO vs GRN vs Invoice)
    // ─────────────────────────────────────────────
    qty_ordered  = po_line.qty
    qty_received = grn_line.qty_accepted  // only accepted qty
    qty_invoiced = invoice_line.qty
    
    IF qty_invoiced > qty_received THEN
        match_result.flags.append("OVER_INVOICE")
        match_result.discrepancies.append({
            type: "QUANTITY",
            severity: "ERROR",
            detail: "Invoiced qty exceeds received qty"
        })
        match_result.status = "FAIL"
    END IF
    
    IF qty_invoiced < qty_received AND variance > tolerance THEN
        match_result.flags.append("UNDER_INVOICE")
        match_result.discrepancies.append({
            type: "QUANTITY",
            severity: "WARN",
            detail: "Invoiced qty less than received qty"
        })
    END IF
    
    // ─────────────────────────────────────────────
    // LEVEL 2: PRICE MATCH (PO vs Invoice)
    // ─────────────────────────────────────────────
    unit_price_po      = po_line.unit_price
    unit_price_invoice = invoice_line.unit_price
    price_tolerance    = global_settings.price_tolerance_pct  // e.g. 5%
    
    price_variance_pct = ABS(unit_price_po - unit_price_invoice) / unit_price_po * 100
    
    IF price_variance_pct > price_tolerance THEN
        match_result.flags.append("PRICE_MISMATCH")
        match_result.discrepancies.append({
            type: "PRICE",
            severity: "ERROR",
            detail: "Unit price variance: " + price_variance_pct + "%"
        })
        match_result.status = "FAIL"
    END IF
    
    // ─────────────────────────────────────────────
    // LEVEL 3: TAX MATCH (PO vs Invoice)
    // ─────────────────────────────────────────────
    IF po_line.tax_code != invoice_line.tax_code THEN
        match_result.flags.append("TAX_MISMATCH")
        match_result.discrepancies.append({
            type: "TAX",
            severity: "ERROR",
            detail: "Tax code mismatch"
        })
        match_result.status = "FAIL"
    END IF
    
    // ─────────────────────────────────────────────
    // LEVEL 4: TOTAL MATCH (GRN Qty × PO Price vs Invoice)
    // ─────────────────────────────────────────────
    expected_amount = qty_received * unit_price_po
    actual_amount   = invoice_line.net_amount
    amount_tolerance = global_settings.amount_tolerance
    
    IF ABS(expected_amount - actual_amount) > amount_tolerance THEN
        match_result.flags.append("AMOUNT_MISMATCH")
        match_result.discrepancies.append({
            type: "AMOUNT",
            severity: "WARN",
            detail: "Expected: " + expected_amount + ", Invoiced: " + actual_amount
        })
        // Does not auto-fail; goes to manual review
    END IF
    
    // ─────────────────────────────────────────────
    // LEVEL 5: LINE-LEVEL vs HEADER-LEVEL CHARGES
    // ─────────────────────────────────────────────
    // Freight, handling, discounts checked separately
    IF po.header_charges != invoice.header_charges THEN
        match_result.flags.append("CHARGE_MISMATCH")
    END IF
    
    // ─────────────────────────────────────────────
    // DECISION
    // ─────────────────────────────────────────────
    IF match_result.status == "FAIL" THEN
        Route for Manual Review
        Set Invoice status = "BLOCKED"
        Notify AP Team
    ELSE IF match_result.flags is not empty THEN
        Route for Manual Review
        Set Invoice status = "ON_HOLD"
        Auto-notify: "Match with warnings"
    ELSE
        Set Invoice status = "MATCHED"
        Auto-approve for payment schedule
    END IF
    
    RETURN match_result
END FUNCTION
```

### Matching Rules Configuration

| Rule | Default Tolerance | Action on Variance |
|------|------------------|-------------------|
| Qty: Invoice > GRN | 0% (strict) | BLOCK payment |
| Qty: Invoice < GRN | 0% | WARN; allow if accepted |
| Price variance | ±5% (configurable per item/supplier) | BLOCK if > tolerance |
| Amount variance (line) | ±$0.50 | WARN |
| Tax code | Must match exactly | BLOCK |
| Currency exchange | ±1% on revalued amount | WARN |
| Freight / charges | Must match PO or have prior approval | BLOCK |
| Discount | Must match PO terms | WARN if different |

### Match States

```
                    ┌────────────┐
                    │  INVOICE   │
                    │  RECEIVED  │
                    └─────┬──────┘
                          │
                          ▼
                 ┌─────────────────┐
         ┌──────┤ 3-Way Matching  ├──────┐
         │      │   Engine Runs    │      │
         │      └────────┬────────┘      │
         ▼                ▼               ▼
   ┌──────────┐    ┌──────────┐    ┌──────────┐
   │ MATCHED  │    │ ON HOLD  │    │ BLOCKED  │
   │ (Auto)   │    │ (Warn)   │    │ (Error)  │
   ├──────────┤    ├──────────┤    ├──────────┤
   │ Payment  │    │ Manual   │    │ Manual   │
   │ Released │    │ Review   │    │ Review   │
   │          │    │ required │    │ required │
   └──────────┘    └──────────┘    └──────────┘
```

---

## 4. Vendor Scorecard Calculation

### Scoring Model (Composite)

```
VENDOR_SCORE = Σ (dimension_weight_i × dimension_score_i) / 100

Dimension Weight Configuration:
┌────────────────────────┬────────┬────────────────────────────┐
│ Dimension              │ Weight │ Data Source                │
├────────────────────────┼────────┼────────────────────────────┤
│ On-Time Delivery (OTD) │  30%   │ GRN vs PO delivery date    │
│ Quality Acceptance     │  25%   │ QI pass rate / reject rate │
│ Pricing Competitiveness│  15%   │ Price variance vs market   │
│ Invoice Accuracy       │  10%   │ 3-way match failure rate   │
│ Lead Time Reliability  │  10%   │ Actual vs promised lead    │
│ Compliance (Docs)      │   5%   │ Doc submission % on time   │
│ Responsiveness         │   5%   │ RFQ/PO ack time; ticket    │
└────────────────────────┴────────┴────────────────────────────┘
```

### Per-Dimension Calculation

#### On-Time Delivery (OTD)

```
OTD_Score = (On-Time Deliveries / Total Deliveries) × 100

Where: On-Time = GRN_date ≤ PO_promised_delivery_date

Tier: Early (±2 days) = 100%, Late ≤3 days = 70%, Late >3 days = 0%
```

#### Quality Acceptance (QA)

```
QA_Score = (1 - Rejected_Qty / Total_Inspected_Qty) × 100

Bonus multiplier: +5% if zero critical defects for 10+ consecutive lots
Penalty: -10% if any safety/critical defect found
```

#### Pricing Competitiveness (PC)

```
PC_Score = 100 - (ABS(vendor_price - best_price) / best_price × 100)

Where best_price = minimum of all quotes for same item in last 90 days
```

#### Invoice Accuracy (IA)

```
IA_Score = (1 - Failed_Matches / Total_Invoices) × 100

Averaged over rolling 6 months
```

### Score Tiers

| Score Range | Tier | Action |
|-------------|------|--------|
| 90–100 | Platinum | Auto-approve RFQ; priority allocation; extended credit terms |
| 75–89 | Gold | Standard processing; preferred routing |
| 60–74 | Silver | Conditional routing; increased QI sampling |
| 40–59 | Bronze | Manual review required; reduced order quantity |
| < 40 | Blacklist | Suspend new orders; review existing contracts |

### Scorecard Periodicity

```
Rolling 12-month window, recalculated monthly
Also available: Per-quarter snapshot for trend analysis
```

### Trend Indicator

```
TREND = (Score_this_month - Score_last_3_month_avg) / Score_last_3_month_avg × 100

If TREND < -10% → "Declining" → trigger procurement review
If TREND > +10% → "Improving"
Else → "Stable"
```

---

## 5. Database Schema (30+ Tables)

### Core Procurement Tables

#### `pur_po_master` — Purchase Order Header
| Column | Type | Description |
|--------|------|-------------|
| po_id | UUID PK | |
| po_number | VARCHAR(20) UNIQUE | Human-readable PO number |
| po_type | ENUM('STANDARD', 'RETURN', 'CONTRACT', 'BLANKET') | |
| status | VARCHAR(20) | Draft, Sent, Ack'd, Confirmed, Partially Received, Fully Received, Closed, Cancelled |
| supplier_id | UUID FK → supplier_master | |
| buyer_id | UUID FK → user | |
| warehouse_id | UUID FK → warehouse | Default receiving location |
| currency | VARCHAR(3) | ISO code |
| exchange_rate | DECIMAL(12,6) | |
| payment_term_id | UUID FK → payment_terms | |
| delivery_term_id | UUID FK → delivery_terms | |
| shipping_method | VARCHAR(50) | |
| expected_delivery_date | DATE | |
| promised_delivery_date | DATE | From supplier confirmation |
| actual_delivery_date | DATE | Last GRN date |
| sub_total | DECIMAL(18,2) | |
| discount_amount | DECIMAL(18,2) | |
| tax_amount | DECIMAL(18,2) | |
| freight_amount | DECIMAL(18,2) | |
| grand_total | DECIMAL(18,2) | |
| notes | TEXT | |
| acknowledgement_date | DATETIME | Supplier ack timestamp |
| cancelled_date | DATETIME | |
| cancel_reason | VARCHAR(255) | |
| created_by | UUID FK → user | |
| created_at | DATETIME | |
| updated_at | DATETIME | |

#### `pur_po_item` — Purchase Order Lines
| Column | Type | Description |
|--------|------|-------------|
| po_item_id | UUID PK | |
| po_id | UUID FK → pur_po_master | |
| line_no | INT | Sequence within PO |
| item_id | UUID FK → item_master | |
| item_code | VARCHAR(50) | Denormalized for performance |
| item_name | VARCHAR(255) | Snapshot at time of PO |
| uom_id | UUID FK → uom | |
| ordered_qty | DECIMAL(18,3) | |
| received_qty | DECIMAL(18,3) | Cumulative from GRN |
| accepted_qty | DECIMAL(18,3) | QI-passed qty |
| rejected_qty | DECIMAL(18,3) | QI-failed qty |
| returned_qty | DECIMAL(18,3) | Via debit notes |
| outstanding_qty | DECIMAL(18,3) | Computed: ordered - received |
| unit_price | DECIMAL(18,4) | |
| discount_percent | DECIMAL(5,2) | |
| tax_code_id | UUID FK → tax_code | |
| tax_amount | DECIMAL(18,2) | |
| net_amount | DECIMAL(18,2) | |
| required_date | DATE | |
| promised_date | DATE | Supplier committed |
| blanket_line_ref | UUID | Reference to blanket order |
| status | VARCHAR(20) | Open, Received, Closed, Cancelled |
| inspection_required | BOOLEAN | |
| is_backorder | BOOLEAN | TRUE if from backorder flow |

#### `pur_po_approval` — PO Approval Trail
| Column | Type | Description |
|--------|------|-------------|
| approval_id | UUID PK | |
| po_id | UUID FK → pur_po_master | |
| approval_level | INT | Sequential approval step |
| approver_id | UUID FK → user | |
| status | ENUM('PENDING', 'APPROVED', 'REJECTED') | |
| comments | TEXT | |
| acted_at | DATETIME | |

#### `pur_po_history` — State Change Audit
| Column | Type | Description |
|--------|------|-------------|
| history_id | UUID PK | |
| po_id | UUID FK → pur_po_master | |
| from_status | VARCHAR(20) | |
| to_status | VARCHAR(20) | |
| changed_by | UUID FK → user | Or 'SYSTEM' for auto |
| change_reason | TEXT | |
| created_at | DATETIME | |

### Goods Receipt Tables

#### `pur_grn_master` — Goods Receipt Note Header
| Column | Type | Description |
|--------|------|-------------|
| grn_id | UUID PK | |
| grn_number | VARCHAR(20) UNIQUE | |
| po_id | UUID FK → pur_po_master | |
| supplier_id | UUID FK → supplier_master | |
| warehouse_id | UUID FK → warehouse | |
| receipt_date | DATETIME | |
| delivery_challan_no | VARCHAR(50) | Supplier document |
| vehicle_no | VARCHAR(20) | |
| driver_name | VARCHAR(100) | |
| driver_contact | VARCHAR(20) | |
| total_pallets | INT | |
| status | ENUM('DRAFT', 'PENDING_QI', 'QI_IN_PROGRESS', 'COMPLETED', 'CANCELLED') | |
| is_qi_blocked | BOOLEAN | TRUE if pending inspection |
| notes | TEXT | |
| created_by | UUID FK → user | |
| created_at | DATETIME | |

#### `pur_grn_item` — GRN Lines
| Column | Type | Description |
|--------|------|-------------|
| grn_item_id | UUID PK | |
| grn_id | UUID FK → pur_grn_master | |
| po_item_id | UUID FK → pur_po_item | |
| item_id | UUID FK → item_master | |
| received_qty | DECIMAL(18,3) | Quantity at gate |
| accepted_qty | DECIMAL(18,3) | Post-QI acceptance |
| rejected_qty | DECIMAL(18,3) | Post-QI rejection |
| return_qty | DECIMAL(18,3) | Actually returned |
| uom_id | UUID FK → uom | |
| unit_price | DECIMAL(18,4) | Carried from PO |
| batch_no | VARCHAR(50) | Supplier batch/lot |
| expiry_date | DATE | For perishables |
| mfg_date | DATE | |
| storage_location | VARCHAR(50) | Rack/bin within warehouse |
| inspection_status | ENUM('PENDING', 'PASS', 'FAIL', 'CONDITIONAL') | |
| inspection_id | UUID FK → pur_qi_inspection | |
| rejection_reason | TEXT | |
| line_total | DECIMAL(18,2) | |

#### `pur_grn_serial_batch` — Batch/Serial Tracking at GRN
| Column | Type | Description |
|--------|------|-------------|
| grn_batch_id | UUID PK | |
| grn_item_id | UUID FK → pur_grn_item | |
| batch_no | VARCHAR(50) | Supplier lot number |
| internal_batch_no | VARCHAR(50) | System-generated |
| serial_no | VARCHAR(100) | For serialized items |
| qty | DECIMAL(18,3) | |
| mfg_date | DATE | |
| expiry_date | DATE | |
| cost_price | DECIMAL(18,4) | |

### Quality Inspection Tables

#### `pur_qi_inspection` — Quality Inspection Header
| Column | Type | Description |
|--------|------|-------------|
| inspection_id | UUID PK | |
| inspection_number | VARCHAR(20) UNIQUE | |
| grn_id | UUID FK → pur_grn_master | |
| grn_item_id | UUID FK → pur_grn_item | |
| qi_template_id | UUID FK → pur_qi_template | |
| inspection_type | ENUM('INCOMING', 'IN_PROCESS', 'OUTGOING') | |
| sample_size | INT | |
| inspector_id | UUID FK → user | |
| verified_by | UUID FK → user | |
| inspection_date | DATETIME | |
| status | ENUM('DRAFT', 'IN_PROGRESS', 'COMPLETED', 'CANCELLED') | |
| result | ENUM('ACCEPTED', 'REJECTED', 'CONDITIONAL', 'HOLD') | Usage decision |
| quality_score | DECIMAL(5,2) | 0.00–100.00 |
| ud_code | VARCHAR(10) | Usage decision code (A/R/H) |
| stock_disposition | ENUM('UNRESTRICTED', 'BLOCKED', 'SCRAP', 'QUARANTINE') | |
| notes | TEXT | |
| created_at | DATETIME | |

#### `pur_qi_reading` — Inspection Parameter Readings
| Column | Type | Description |
|--------|------|-------------|
| reading_id | UUID PK | |
| inspection_id | UUID FK → pur_qi_inspection | |
| parameter_id | UUID FK → pur_qi_parameter | |
| reading_value | VARCHAR(255) | Numeric or text |
| reading_1 | DECIMAL(18,4) | Numeric reading 1 |
| reading_2 | DECIMAL(18,4) | Numeric reading 2 |
| reading_3 | DECIMAL(18,4) | Numeric reading 3 |
| is_within_spec | BOOLEAN | |
| status | ENUM('PASS', 'FAIL', 'MANUAL') | |
| manual_inspection | BOOLEAN | Override flag |
| notes | TEXT | |

#### `pur_qi_template` — Inspection Template
| Column | Type | Description |
|--------|------|-------------|
| template_id | UUID PK | |
| template_name | VARCHAR(100) | |
| item_id | UUID FK → item_master | Optional: item-specific plan |
| supplier_id | UUID FK → supplier_master | Optional: supplier-specific plan |
| category | VARCHAR(50) | |
| is_active | BOOLEAN | |

#### `pur_qi_parameter` — Inspection Parameters
| Column | Type | Description |
|--------|------|-------------|
| parameter_id | UUID PK | |
| template_id | UUID FK → pur_qi_template | |
| parameter_name | VARCHAR(100) | |
| parameter_type | ENUM('NUMERIC', 'NON_NUMERIC', 'FORMULA') | |
| min_value | DECIMAL(18,4) | For numeric |
| max_value | DECIMAL(18,4) | For numeric |
| acceptance_value | VARCHAR(255) | For non-numeric |
| formula | TEXT | For formula-based |
| is_critical | BOOLEAN | If fail → auto-reject lot |
| sequence | INT | |

### Reorder & Automation Tables

#### `pur_reorder_rule` — Reorder Configuration
| Column | Type | Description |
|--------|------|-------------|
| rule_id | UUID PK | |
| item_id | UUID FK → item_master | |
| warehouse_id | UUID FK → warehouse | |
| reorder_level | DECIMAL(18,3) | Min stock trigger |
| reorder_qty | DECIMAL(18,3) | Quantity to order |
| max_qty | DECIMAL(18,3) | Max stock (Odoo style) |
| safety_stock | DECIMAL(18,3) | |
| lead_time_days | INT | Supplier lead time |
| preferred_supplier_id | UUID FK → supplier_master | |
| material_request_type | ENUM('PURCHASE', 'TRANSFER') | |
| trigger | ENUM('AUTO', 'MANUAL') | |
| is_active | BOOLEAN | |
| last_triggered_at | DATETIME | |

#### `pur_material_request` — Material Request Header
| Column | Type | Description |
|--------|------|-------------|
| mr_id | UUID PK | |
| mr_number | VARCHAR(20) UNIQUE | |
| mr_type | ENUM('PURCHASE', 'TRANSFER') | |
| status | ENUM('DRAFT', 'SUBMITTED', 'PARTIALLY_ORDERED', 'ORDERED', 'RECEIVED', 'CANCELLED', 'STOPPED') | |
| auto_created | BOOLEAN | TRUE if from reorder rule |
| company_id | UUID FK → company | |
| transaction_date | DATE | |
| created_by | UUID FK → user | |
| notes | TEXT | |

#### `pur_material_request_item` — Material Request Lines
| Column | Type | Description |
|--------|------|-------------|
| mri_id | UUID PK | |
| mr_id | UUID FK → pur_material_request | |
| item_id | UUID FK → item_master | |
| qty | DECIMAL(18,3) | |
| uom_id | UUID FK → uom | |
| warehouse_id | UUID FK → warehouse | |
| required_date | DATE | |
| projected_on_hand | DECIMAL(18,3) | Snapshot at creation |
| reorder_level | DECIMAL(18,3) | Snapshot at creation |
| reorder_qty | DECIMAL(18,3) | Snapshot at creation |
| status | ENUM('PENDING', 'ORDERED', 'RECEIVED', 'CANCELLED') | |

### Vendor Management Tables

#### `vendor_master` — Vendor Master
| Column | Type | Description |
|--------|------|-------------|
| vendor_id | UUID PK | |
| vendor_code | VARCHAR(20) UNIQUE | |
| vendor_name | VARCHAR(255) | |
| legal_name | VARCHAR(255) | |
| vendor_type | ENUM('INDIVIDUAL', 'COMPANY', 'GOVERNMENT') | |
| tax_id | VARCHAR(50) | GST/VAT/Tax ID |
| pan_no | VARCHAR(20) | |
| registration_no | VARCHAR(50) | Company registration |
| credit_limit | DECIMAL(18,2) | |
| credit_days | INT | |
| payment_term_id | UUID FK → payment_terms | |
| currency | VARCHAR(3) | Default currency |
| payment_method | VARCHAR(50) | |
| is_active | BOOLEAN | |
| blacklisted | BOOLEAN | |
| blacklist_reason | TEXT | |
| onboarding_status | ENUM('PENDING', 'APPROVED', 'REJECTED', 'SUSPENDED') | |
| self_registered | BOOLEAN | Via vendor portal |
| portal_user_id | VARCHAR(100) | Link to portal auth |
| current_score | DECIMAL(5,2) | Latest vendor score |
| score_tier | VARCHAR(20) | Platinum/Gold/Silver/Bronze |
| notes | TEXT | |
| created_at | DATETIME | |

#### `vendor_contact` — Vendor Contacts
| Column | Type | Description |
|--------|------|-------------|
| contact_id | UUID PK | |
| vendor_id | UUID FK → vendor_master | |
| contact_type | ENUM('PRIMARY', 'SALES', 'ACCOUNTS', 'DISPATCH') | |
| salutation | VARCHAR(10) | |
| first_name | VARCHAR(100) | |
| last_name | VARCHAR(100) | |
| email | VARCHAR(255) | |
| phone | VARCHAR(20) | |
| mobile | VARCHAR(20) | |
| designation | VARCHAR(100) | |
| is_active | BOOLEAN | |

#### `vendor_address` — Vendor Addresses
| Column | Type | Description |
|--------|------|-------------|
| address_id | UUID PK | |
| vendor_id | UUID FK → vendor_master | |
| address_type | ENUM('BILLING', 'SHIPPING', 'REGISTERED', 'WAREHOUSE') | |
| address_line_1 | VARCHAR(255) | |
| address_line_2 | VARCHAR(255) | |
| city | VARCHAR(100) | |
| state | VARCHAR(100) | |
| postal_code | VARCHAR(20) | |
| country | VARCHAR(100) | |
| geo_lat | DECIMAL(10,7) | For proximity routing |
| geo_lng | DECIMAL(10,7) | |
| is_default | BOOLEAN | |

#### `vendor_bank_account` — Bank Details
| Column | Type | Description |
|--------|------|-------------|
| bank_id | UUID PK | |
| vendor_id | UUID FK → vendor_master | |
| bank_name | VARCHAR(255) | |
| branch | VARCHAR(255) | |
| account_no | VARCHAR(50) | |
| account_type | VARCHAR(50) | |
| ifsc_code | VARCHAR(20) | |
| swift_code | VARCHAR(20) | |
| is_default | BOOLEAN | |

#### `vendor_category` — Vendor Commodity/Category Mapping
| Column | Type | Description |
|--------|------|-------------|
| vc_id | UUID PK | |
| vendor_id | UUID FK → vendor_master | |
| category_id | UUID FK → item_category | |
| is_preferred | BOOLEAN | |

#### `vendor_contract` — Price/Contract Agreements
| Column | Type | Description |
|--------|------|-------------|
| contract_id | UUID PK | |
| vendor_id | UUID FK → vendor_master | |
| contract_no | VARCHAR(50) | |
| contract_type | ENUM('RATE_CONTRACT', 'QUANTITY_CONTRACT', 'BLANKET') | |
| start_date | DATE | |
| end_date | DATE | |
| terms | TEXT | |
| status | ENUM('ACTIVE', 'EXPIRED', 'TERMINATED', 'DRAFT') | |
| created_by | UUID FK → user | |

#### `vendor_price_list` — Supplier Price List
| Column | Type | Description |
|--------|------|-------------|
| price_id | UUID PK | |
| vendor_id | UUID FK → vendor_master | |
| item_id | UUID FK → item_master | |
| uom_id | UUID FK → uom | |
| unit_price | DECIMAL(18,4) | |
| currency | VARCHAR(3) | |
| min_order_qty | DECIMAL(18,3) | |
| lead_time_days | INT | |
| effective_from | DATE | |
| effective_to | DATE | |
| is_active | BOOLEAN | |
| priority | INT | Lower = preferred |
| created_at | DATETIME | |

#### `vendor_scorecard` — Performance Score History
| Column | Type | Description |
|--------|------|-------------|
| scorecard_id | UUID PK | |
| vendor_id | UUID FK → vendor_master | |
| period_start | DATE | |
| period_end | DATE | |
| otd_score | DECIMAL(5,2) | |
| quality_score | DECIMAL(5,2) | |
| pricing_score | DECIMAL(5,2) | |
| invoice_accuracy_score | DECIMAL(5,2) | |
| lead_time_score | DECIMAL(5,2) | |
| compliance_score | DECIMAL(5,2) | |
| responsiveness_score | DECIMAL(5,2) | |
| composite_score | DECIMAL(5,2) | |
| tier | VARCHAR(20) | |
| total_deliveries | INT | |
| total_rejected_qty | DECIMAL(18,3) | |
| total_invoices | INT | |
| failed_matches | INT | |
| created_at | DATETIME | |

### Invoice & 3-Way Matching Tables

#### `pur_invoice` — Vendor Invoice (from supplier or AP entry)
| Column | Type | Description |
|--------|------|-------------|
| invoice_id | UUID PK | |
| invoice_no | VARCHAR(50) | Supplier's invoice number |
| invoice_date | DATE | |
| po_id | UUID FK → pur_po_master | |
| vendor_id | UUID FK → vendor_master | |
| invoice_type | ENUM('STANDARD', 'CREDIT_NOTE', 'DEBIT_NOTE') | |
| currency | VARCHAR(3) | |
| exchange_rate | DECIMAL(12,6) | |
| gross_amount | DECIMAL(18,2) | |
| discount_amount | DECIMAL(18,2) | |
| tax_amount | DECIMAL(18,2) | |
| net_amount | DECIMAL(18,2) | |
| outstanding_amount | DECIMAL(18,2) | |
| match_status | ENUM('PENDING', 'MATCHED', 'ON_HOLD', 'BLOCKED') | |
| payment_status | ENUM('UNPAID', 'PAID', 'PARTIAL', 'OVERPAID') | |
| due_date | DATE | |
| notes | TEXT | |
| created_at | DATETIME | |

#### `pur_invoice_item` — Invoice Lines
| Column | Type | Description |
|--------|------|-------------|
| inv_item_id | UUID PK | |
| invoice_id | UUID FK → pur_invoice | |
| po_item_id | UUID FK → pur_po_item | |
| grn_item_id | UUID FK → pur_grn_item | Optional |
| item_id | UUID FK → item_master | |
| qty | DECIMAL(18,3) | |
| uom_id | UUID FK → uom | |
| unit_price | DECIMAL(18,4) | |
| tax_code_id | UUID FK → tax_code | |
| tax_amount | DECIMAL(18,2) | |
| net_amount | DECIMAL(18,2) | |
| match_result | ENUM('PASS', 'FAIL', 'WARN', 'PENDING') | |

#### `pur_three_way_match_log` — Match Audit Log
| Column | Type | Description |
|--------|------|-------------|
| match_id | UUID PK | |
| invoice_id | UUID FK → pur_invoice | |
| invoice_item_id | UUID FK → pur_invoice_item | |
| po_item_id | UUID FK → pur_po_item | |
| grn_item_id | UUID FK → pur_grn_item | |
| qty_ordered | DECIMAL(18,3) | |
| qty_received | DECIMAL(18,3) | |
| qty_invoiced | DECIMAL(18,3) | |
| price_po | DECIMAL(18,4) | |
| price_invoice | DECIMAL(18,4) | |
| tax_po_id | UUID | |
| tax_invoice_id | UUID | |
| flags | JSON | Array of discrepancy flags |
| match_status | ENUM('PASS', 'FAIL', 'WARN') | |
| reviewed_by | UUID FK → user | Null if auto-pass |
| review_notes | TEXT | |
| matched_at | DATETIME | |

### Debit Note / Returns Tables

#### `pur_debit_note` — Debit Note Header
| Column | Type | Description |
|--------|------|-------------|
| debit_note_id | UUID PK | |
| debit_note_no | VARCHAR(20) UNIQUE | |
| grn_id | UUID FK → pur_grn_master | |
| po_id | UUID FK → pur_po_master | |
| vendor_id | UUID FK → vendor_master | |
| return_type | ENUM('QUALITY_REJECT', 'OVER_SHIPMENT', 'WRONG_ITEM', 'EXPIRED', 'DAMAGED', 'BUYBACK') | |
| status | ENUM('DRAFT', 'APPROVED', 'GOODS_RETURNED', 'CLOSED', 'CANCELLED', 'REJECTED') | |
| total_amount | DECIMAL(18,2) | |
| tax_adjustment | DECIMAL(18,2) | |
| net_adjustment | DECIMAL(18,2) | |
| supplier_credit_note_ref | VARCHAR(50) | Supplier's credit note |
| notes | TEXT | |
| created_at | DATETIME | |

#### `pur_debit_note_item` — Debit Note Lines
| Column | Type | Description |
|--------|------|-------------|
| dn_item_id | UUID PK | |
| debit_note_id | UUID FK → pur_debit_note | |
| grn_item_id | UUID FK → pur_grn_item | |
| item_id | UUID FK → item_master | |
| return_qty | DECIMAL(18,3) | |
| uom_id | UUID FK → uom | |
| unit_price | DECIMAL(18,4) | |
| line_total | DECIMAL(18,2) | |
| batch_no | VARCHAR(50) | |
| return_reason | TEXT | |

### Vendor Portal & Communication Tables

#### `vendor_portal_user` — Portal User Credentials
| Column | Type | Description |
|--------|------|-------------|
| portal_user_id | UUID PK | |
| vendor_id | UUID FK → vendor_master | |
| email | VARCHAR(255) UNIQUE | |
| password_hash | VARCHAR(255) | |
| last_login | DATETIME | |
| is_active | BOOLEAN | |
| two_factor_enabled | BOOLEAN | |

#### `vendor_portal_session` — Session Log
| Column | Type | Description |
|--------|------|-------------|
| session_id | UUID PK | |
| portal_user_id | UUID FK → vendor_portal_user | |
| ip_address | VARCHAR(45) | |
| user_agent | TEXT | |
| logged_in_at | DATETIME | |
| last_activity | DATETIME | |
| logged_out_at | DATETIME | |

#### `vendor_asn` — Advance Shipment Notice
| Column | Type | Description |
|--------|------|-------------|
| asn_id | UUID PK | |
| po_id | UUID FK → pur_po_master | |
| vendor_id | UUID FK → vendor_master | |
| asn_number | VARCHAR(50) | Supplier's ASN ref |
| expected_delivery_date | DATE | |
| vehicle_no | VARCHAR(20) | |
| transporter | VARCHAR(100) | |
| driver_name | VARCHAR(100) | |
| driver_contact | VARCHAR(20) | |
| total_packages | INT | |
| gross_weight | DECIMAL(18,3) | |
| status | ENUM('SUBMITTED', 'IN_TRANSIT', 'DELIVERED', 'PARTIAL') | |
| created_at | DATETIME | |

#### `vendor_asn_item` — ASN Lines
| Column | Type | Description |
|--------|------|-------------|
| asn_item_id | UUID PK | |
| asn_id | UUID FK → vendor_asn | |
| po_item_id | UUID FK → pur_po_item | |
| item_id | UUID FK → item_master | |
| expected_qty | DECIMAL(18,3) | |
| batch_no | VARCHAR(50) | |
| expiry_date | DATE | |

#### `vendor_ticket` — Vendor Communication/Support
| Column | Type | Description |
|--------|------|-------------|
| ticket_id | UUID PK | |
| vendor_id | UUID FK → vendor_master | |
| ticket_type | ENUM('PO_QUERY', 'INVOICE_QUERY', 'PAYMENT', 'GRN', 'QUALITY', 'DOCUMENT', 'OTHER') | |
| subject | VARCHAR(255) | |
| description | TEXT | |
| priority | ENUM('LOW', 'MEDIUM', 'HIGH', 'URGENT') | |
| status | ENUM('OPEN', 'IN_PROGRESS', 'WAITING_VENDOR', 'WAITING_BUYER', 'RESOLVED', 'CLOSED') | |
| assigned_to | UUID FK → user | |
| created_by | VARCHAR(255) | Portal user or internal |
| resolved_at | DATETIME | |
| created_at | DATETIME | |

### Fulfillment & Routing Tables

#### `pur_order_split` — Multi-Vendor Order Split
| Column | Type | Description |
|--------|------|-------------|
| split_id | UUID PK | |
| source_po_id | UUID FK → pur_po_master | Original PO that was split |
| destination_po_id | UUID FK → pur_po_master | Resulting child PO |
| item_id | UUID FK → item_master | |
| qty | DECIMAL(18,3) | |
| vendor_id | UUID FK → vendor_master | |
| split_reason | VARCHAR(255) | |
| created_at | DATETIME | |

#### `pur_backorder` — Backorder Tracking
| Column | Type | Description |
|--------|------|-------------|
| backorder_id | UUID PK | |
| source_order_type | ENUM('SALES_ORDER', 'TRANSFER_ORDER') | |
| source_order_id | UUID | Polymorphic FK |
| source_order_line_id | UUID | |
| item_id | UUID FK → item_master | |
| qty_backordered | DECIMAL(18,3) | |
| qty_fulfilled | DECIMAL(18,3) | |
| po_id | UUID FK → pur_po_master | Generated PO |
| po_item_id | UUID FK → pur_po_item | |
| status | ENUM('PENDING', 'PARTIAL', 'FULFILLED', 'CANCELLED') | |
| created_at | DATETIME | |

---

## 6. Must-Have Features with Priority Ranking

| Priority | Feature | Category | Justification |
|----------|---------|----------|---------------|
| **P0** | **PO Creation with full state machine** | Core | Without this, no procurement is possible |
| **P0** | **GRN with quantity verification** | Core | Entry point for inventory; stock accuracy |
| **P0** | **Three-way matching (PO ↔ GRN ↔ Invoice)** | Finance | Prevents overpayment; SOX compliance core |
| **P0** | **Vendor master with tax/bank info** | Master Data | Foundation for all vendor transactions |
| **P0** | **Reorder level → auto MR/PO** | Automation | Prevents stockouts in supermarket |
| **P1** | **Quality inspection with accept/reject** | Quality | Critical for perishables, expiry management |
| **P1** | **Multi-warehouse routing** | Fulfillment | Supermarket chains have multiple stores |
| **P1** | **Partial receipt handling** | Logistics | Rarely does full PO arrive in one shipment |
| **P1** | **Batch/lot number tracking** | Traceability | Regulatory for food & FMCG |
| **P1** | **Vendor performance scorecard** | Vendor Mgmt | Drives procurement decisions |
| **P1** | **Supplier price list management** | Pricing | Negotiated rates; auto-populate PO |
| **P1** | **Backorder management** | Fulfillment | Out-of-stock but committed to customers |
| **P2** | **Debit note / purchase returns** | Finance | Necessary for quality rejections |
| **P2** | **Vendor self-service portal** | Experience | Reduces AP/PO calls by 60%+ |
| **P2** | **Multi-vendor order splitting** | Routing | Best price/speed via vendor split |
| **P2** | **ASN (Advance Shipment Notice)** | Logistics | Warehouse planning; gate scheduling |
| **P2** | **PO approval workflow** | Control | Approval by amount/commodity |
| **P2** | **Contract management (blanket PO)** | Pricing | Long-term agreements with suppliers |
| **P3** | **RFQ / quotation comparison** | Sourcing | Competitive bidding for non-contract items |
| **P3** | **Dynamic QI sampling (skip-lot)** | Optimization | Reduces inspection costs for trusted vendors |
| **P3** | **EDI/API integration for large suppliers** | Integration | Automated PO/ASN/Invoice exchange |
| **P3** | **Reverse auctions** | Sourcing | For commodity categories |
| **P3** | **OCR for supplier invoices** | Automation | Reduces manual AP data entry |
| **P3** | **GST/e-invoice compliance** | Compliance | Country-specific regulatory |
| **P3** | **Procurement dashboard & analytics** | Visibility | Spend analysis, savings tracking |

---

## 7. Reorder-to-PO Automation Flow

### Complete Flow Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        REORDER-TO-PO PIPELINE                           │
│                                                                         │
│  ┌──────────┐     ┌───────────┐     ┌──────────┐     ┌───────────────┐ │
│  │  Scheduler │────▶  Check    │────▶  Below   │────▶  Create MR     │ │
│  │  (Daily/   │     │ Projected │     │ Reorder  │     │ (Draft→Submit) │ │
│  │  Per-Event)│     │ Qty       │     │ Level?   │     └───────┬───────┘ │
│  └──────────┘     └───────────┘     └──────────┘             │         │
│        │                                                        │         │
│        ▼                                                        ▼         │
│  ┌──────────┐                                           ┌───────────────┐ │
│  │ SO Created│                                           │ Notify Purchase│ │
│  │ (ATP Chk) │                                           │ Manager       │ │
│  └──────────┘                                           └───────┬───────┘ │
│        │                                                        │         │
│        ▼                                                        ▼         │
│  ┌──────────┐                                           ┌───────────────┐ │
│  │Stock     │                                           │ Convert MR    │ │
│  │Movement  │                                           │ to RFQ / PO   │ │
│  │(Sales)   │                                           └───────┬───────┘ │
│  └──────────┘                                                   │         │
│         │                                                       ▼         │
│         └───────────────────┬──────────────────────────────────┘          │
│                             ▼                                            │
│                    ┌──────────────────┐                                  │
│                    │  Select Vendor   │                                  │
│                    │  (Routing Engine)│                                  │
│                    └────────┬─────────┘                                  │
│                             │                                            │
│                ┌────────────┴────────────┐                               │
│                ▼                         ▼                               │
│       ┌──────────────────┐    ┌──────────────────┐                      │
│       │ Single Vendor    │    │ Multi-Vendor Split│                     │
│       │ (Preferred/      │    │ (By item/category │                     │
│       │  Default)        │    │  /qty/cost)       │                     │
│       └────────┬─────────┘    └────────┬─────────┘                      │
│                │                       │                                │
│                └───────────┬───────────┘                                │
│                            ▼                                            │
│                   ┌──────────────────┐                                  │
│                   │  Create PO Draft │                                  │
│                   │  Auto-fill:      │                                  │
│                   │  - Item/Price    │                                  │
│                   │  - Lead Time     │                                  │
│                   │  - Delivery Date │                                  │
│                   │  - Warehouse     │                                  │
│                   └────────┬─────────┘                                  │
│                            │                                            │
│                            ▼                                            │
│                   ┌──────────────────┐                                  │
│                   │  Auto-approve?   │                                  │
│                   │  (If PO amount   │                                  │
│                   │   < threshold)   │                                  │
│                   └───┬──────┬───────┘                                  │
│                       │      │                                          │
│                       ▼      ▼                                          │
│                  Submit   Route for                                    │
│                  & Send   Approval                                      │
│                  to Vendor                                              │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### Reorder Rule Evaluation Logic (Pseudocode)

```python
def evaluate_reorder_rules():
    """
    Runs nightly or on-demand. Based on ERPNext _reorder_item() logic.
    """
    rules = get_active_reorder_rules()
    
    for rule in rules:
        item = get_item(rule.item_id)
        warehouse = rule.warehouse
        
        # Compute projected qty: on_hand + incoming - outgoing
        projected_qty = get_projected_qty(item, warehouse)
        
        if projected_qty <= rule.reorder_level:
            # Deficiency = reorder_level - projected_qty
            deficiency = rule.reorder_level - projected_qty
            
            # Reorder qty = max(rule.reorder_qty, deficiency)
            order_qty = max(rule.reorder_qty, deficiency)
            
            # Cap at max_qty if configured
            if rule.max_qty:
                order_qty = min(order_qty, rule.max_qty - projected_qty)
            
            # Check existing open POs/MRs for same item
            pending_qty = get_pending_po_qty(item, warehouse)
            order_qty -= pending_qty
            
            if order_qty <= 0:
                continue  # Already ordered enough
            
            # Create Material Request
            mr = create_material_request(
                item=item,
                qty=order_qty,
                warehouse=warehouse,
                rule=rule,
                auto_created=True
            )
            
            # If trigger == AUTO, proceed to PO creation
            if rule.trigger == 'AUTO':
                # Route to best vendor
                vendor = select_best_vendor(item, rule, qty=order_qty)
                
                if vendor:
                    po = create_purchase_order(
                        mr=mr,
                        vendor=vendor,
                        item=item,
                        qty=order_qty,
                        delivery_date=now() + timedelta(days=vendor.lead_time_days)
                    )
                    
                    # Auto-submit if below approval threshold
                    if po.grand_total <= get_auto_approval_threshold():
                        po.submit()
                        po.send_to_vendor()
                    else:
                        po.submit_draft()
                        notify_approvers(po)
```

### Vendor Routing Decision (Multi-Vendor)

```python
def select_best_vendor(item, rule, qty):
    """
    Scores available vendors and selects optimal.
    Order Splitting Logic included.
    """
    vendors = get_qualified_vendors(item, rule)
    
    if not vendors:
        return None
    
    if len(vendors) == 1:
        return vendors[0]
    
    # Score each vendor
    scored = []
    for v in vendors:
        score = (
            v.current_score * 0.4 +                       # Performance score
            (1 - v.price_list.unit_price / max_price) * 0.3 +  # Price competitiveness
            (1 - v.lead_time_days / max_lead_time) * 0.2 +    # Speed
            (1 if v.is_preferred else 0) * 0.1                # Preference bonus
        )
        scored.append((score, v))
    
    scored.sort(key=lambda x: x[0], reverse=True)
    
    # Check if order splitting is needed (qty exceeds single vendor capacity)
    top_vendor = scored[0][1]
    if top_vendor.max_order_qty and qty > top_vendor.max_order_qty:
        return split_order_across_vendors(scored, qty)
    
    return top_vendor


def split_order_across_vendors(scored_vendors, total_qty):
    """
    Splits order quantity across multiple vendors.
    Creates separate POs for each vendor.
    """
    remaining_qty = total_qty
    splits = []
    
    for score, vendor in scored_vendors:
        if remaining_qty <= 0:
            break
        
        alloc_qty = min(vendor.max_order_qty or remaining_qty, remaining_qty)
        if alloc_qty >= vendor.min_order_qty:
            splits.append((vendor, alloc_qty))
            remaining_qty -= alloc_qty
    
    if remaining_qty > 0:
        # Fallback: give remaining to top vendor (partial capacity override)
        splits[-1] = (splits[-1][0], splits[-1][1] + remaining_qty)
    
    return splits  # Returns list of (vendor, qty) tuples → one PO each
```

### Scheduler Triggers

| Trigger | Frequency | Action |
|---------|-----------|--------|
| Nightly batch | Daily at 2 AM | Evaluate all reorder rules; create MRs/POs |
| On SO confirmation | Real-time | Check ATP; trigger reorder if stock dips below reorder level |
| On stock adjustment | Real-time | Negative adjustment may trigger reorder |
| Manual run | On-demand | Admin triggers via "Run Scheduler" button |
| Periodic (Odoo style) | Configurable | Runs on scheduled intervals or SO save events |

---

## Key Architectural Patterns from Industry ERPs

| Pattern | SAP | Oracle | ERPNext | Odoo |
|---------|-----|--------|---------|------|
| Reorder trigger | MRP → Purchase Requisition | Min-Max Planning | Reorder Level → Auto MR | Reordering Rules → RFQ |
| PO states | Created → Approved → Printed → GR | Draft → Approved → Open → Closed | Draft → Submitted → To Receive → Closed | RFQ → Purchase Order → Done |
| QI integration | QM module; Inspection lot at GR | Quality module; Accept/Reject | QI linked to PR/Stock Entry | Quality Control Points |
| 3-way match | Automatic via MM-IV | Payables matching engine | Purchase Invoice → PR match | Invoice → Receipt Match |
| Vendor scoring | LIS (Vendor Evaluation) | Supplier Assessment | Supplier Scorecard | Supplier Evaluation |
| Batch tracking | Batch management (MSC1N) | Lot/serial tracking | Batch no in Item/Stock Entry | Lots/Serial Numbers |
| Portal | SAP Supplier Portal | Oracle Supplier Portal | N/A (portal via Frappe) | Vendor Portal module |
| Returns | Movement type 161/122 | Return to Supplier | Purchase Return | Return Order |
| ORM Integration | SAP Ariba, SAP S/4HANA | Oracle Cloud SCM | Built-in Frappe/ERPNext | Odoo Purchase + Inventory |

---

## References & Further Reading

- ERPNext Purchase Order docs: https://docs.erpnext.com/docs/user/manual/en/purchase-order
- ERPNext Auto MR: https://docs.frappe.io/erpnext/auto-creation-of-material-request
- Odoo Reordering Rules: https://www.odoo.com/documentation/19.0/applications/inventory_and_mrp/inventory/warehouses_storage/replenishment/reordering_rules.html
- SAP QM Inspection (QA32): https://hobsoft.com/guides/sap-qm-incoming-raw-material-inspection-procedure-deep-dive-into-qa32/
- SAP Quality Certificates at GR: https://learning.sap.com/learning-journeys/configuring-sap-s-4hana-quality-management/processing-quality-certificates-at-goods-receipt
- SAP Usage Decisions: https://learning.sap.com/learning-journeys/configuring-sap-s-4hana-quality-management/perfoming-usage-decisions
- Infor M3 GR Flow: https://docs.infor.com/m3udi/16.x/en-us/m3beud/prochs/pps350.html
- Microsoft Dynamics 365 Purchase Returns: https://learn.microsoft.com/en-us/dynamics365/supply-chain/procurement/tasks/create-purchase-return-order
- Business Central Order Processing: https://topdynamicspartners.com/learn/business-central/order-processing
- SAP Business One Backorders: https://b1bwise.com/navigating-the-split-managing-partial-shipments-and-backorders-in-sap-business-one/
- SAP S/4HANA Return to Supplier: https://community.sap.com/t5/enterprise-resource-planning-blog-posts-by-sap/return-to-supplier-in-sap-s4hana-cloud-public-edition/ba-p/13688971
- Vendor Portal features (Vendora): https://www.vendorascmsoftware.com/vendor-portal-software/
- Sourced Oracle Supplier Portal: https://www.gosourced.ai/en/oracle-supplier-portal
- Cin7 Core Backorders: https://help.core.cin7.com/hc/en-us/articles/9034480522511-Backorders-and-Split-Orders
- Agiliron Backorder Management: https://learn.agiliron.com/docs/backorder-management-fulfillment
- O2VEND Debit Notes: https://o2vend.com/help/order-to-cash/debit-note-management.html
- Duoplane Order Routing: https://duoplane.com/features/order-routing/
- Logicbroker Intelligent Routing: https://logicbroker.com/features/automated-optimized-order-routing/
